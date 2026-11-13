using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Persistence.Configurations
{
    public class TelegramAuthRequestConfiguration : IEntityTypeConfiguration<TelegramAuthRequest>
    {
        public void Configure(EntityTypeBuilder<TelegramAuthRequest> builder)
        {
            builder.ToTable("TelegramAuthRequests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(r => r.TelegramChatId)
                .IsRequired(false);

            builder.Property(r => r.CodeHash)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(r => r.Attempts)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.ExpiresAt)
                .IsRequired();

            builder.HasIndex(r => r.ExpiresAt);
        }
    }
}
