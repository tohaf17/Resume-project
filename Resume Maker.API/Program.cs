using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Options;
using ResumeMaker.Infrastructure;
using ResumeMaker.Infrastructure.Repositories;
using ResumeMaker.Infrastructure.Services;      
using System.Threading.RateLimiting;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
//конфігурація TelegramBotOptions
builder.Services.Configure<TelegramBotOptions>(
    builder.Configuration.GetSection(TelegramBotOptions.SectionName));

//TelegramWorker registration
builder.Services.AddHostedService<TelegramBotWorker>();
builder.Services.AddScoped<ITelegramAuthRepository, TelegramAuthRepository>();
builder.Services.AddScoped<ITelegramAuthService, TelegramAuthService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        JsonBotAPI.Configure(options.JsonSerializerOptions);
    });


builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Доступно за адресою /swagger
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
