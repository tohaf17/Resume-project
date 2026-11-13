using Microsoft.EntityFrameworkCore;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Infrastructure.Persistence.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Persistence
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TelegramAuthRequest> TelegramAuthRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

            ConfigureRelationships(modelBuilder);
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Один TariffPlan може бути призначений багатьом User (опціональний FK)
            // Розкоментуйте після створення сутності TariffPlan:
            /*
            modelBuilder.Entity<User>()
                .HasOne(u => u.TariffPlan)
                .WithMany()
                .HasForeignKey(u => u.TariffPlanId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            */
        }
    }
}
