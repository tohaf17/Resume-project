using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Enums;
using ResumeMaker.Application.Options;
using ResumeMaker.Infrastructure;
using ResumeMaker.Infrastructure.Persistence;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
//конфігурація підключення до бази даних
builder.Services.AddInfrastructure(builder.Configuration);

//
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
          ?? throw new InvalidOperationException("Jwt section is missing");

if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
    throw new InvalidOperationException(
        "Jwt:Key is not set. Run: dotnet user-secrets set \"Jwt:Key\" \"<key>\"");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();
//конфігурація Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Політика "ip-limit": максимум 5 запитів за 1 хвилину з однієї IP-адреси
    options.AddPolicy("ip-limit", httpContext =>
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ipAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5, // Максимум запитів
                Window = TimeSpan.FromMinutes(1), // Вікно часу
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // 0 = одразу відхиляти зайві запити без черги
            });
    });
});
//конфігурація TelegramBotOptions
builder.Services.Configure<TelegramBotOptions>(
    builder.Configuration.GetSection(TelegramBotOptions.SectionName));

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    var adminEmail = builder.Configuration["AdminBootstrap:Email"];
    var adminPassword = builder.Configuration["AdminBootstrap:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword) &&
        !await dbContext.Users.AnyAsync(u => u.Email == adminEmail))
    {
        dbContext.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = adminEmail,
            PasswordHash = passwordHasher.HashPassword(adminPassword),
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();
    }
    if (!await dbContext.ResumeTemplates.AnyAsync())
    {
        dbContext.ResumeTemplates.AddRange(
            new ResumeTemplate
            {
                Id = Guid.Parse("7c9c7b90-1111-4111-8111-111111111111"),
                Name = "Basic Free Template",
                IsPremium = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ResumeTemplate
            {
                Id = Guid.Parse("7c9c7b90-2222-4222-8222-222222222222"),
                Name = "Executive Premium Template",
                IsPremium = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        await dbContext.SaveChangesAsync();
    }
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
