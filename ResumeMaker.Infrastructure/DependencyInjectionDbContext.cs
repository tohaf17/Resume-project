using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Infrastructure.Persistence;
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
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            return services;
        }
    }
}
