using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Infrastructure.Persistence;
using ResumeMaker.Infrastructure.Repositories;
using ResumeMaker.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITelegramAuthRepository, TelegramAuthRepository>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            services.AddScoped<ITelegramAuthService, TelegramAuthService>();
            services.AddScoped<ILandingService, LandingService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddHostedService<TelegramBotWorker>();
            return services;
        }
    }
}
