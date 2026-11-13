using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.TelegramUserId)
                .IsRequired();

            builder.HasIndex(u => u.TelegramUserId)
                .IsUnique();

            builder.Property(u => u.TelegramUsername)
                .HasMaxLength(64)
                .IsRequired(false);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(u => u.Role)
                .HasConversion<string>() 
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(u => u.TariffPlanId)
                .IsRequired(false);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Зв'язок Many-to-One: User -> TariffPlan (розкоментуйте, коли створите сутність TariffPlan)
            // builder.HasOne(u => u.TariffPlan)
            //     .WithMany(tp => tp.Users)
            //     .HasForeignKey(u => u.TariffPlanId)
            //     .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
