//added
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Infrastructure.Persistence.Configurations;

public sealed class RegistrationPromptEventConfiguration : IEntityTypeConfiguration<RegistrationPromptEvent>
{
    public void Configure(EntityTypeBuilder<RegistrationPromptEvent> builder)
    {
        builder.ToTable("RegistrationPromptEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(e => e.SourcePage)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.TemplateSlug)
            .HasMaxLength(150);

        builder.Property(e => e.AnonymousSessionId)
            .HasMaxLength(128);

        builder.HasIndex(e => e.CreatedAt);
    }
}
