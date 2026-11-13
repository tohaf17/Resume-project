using ResumeMaker.Infrastructure;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//конфігурація підключення до бази даних
builder.Services.AddInfrastructure(builder.Configuration);

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


builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
