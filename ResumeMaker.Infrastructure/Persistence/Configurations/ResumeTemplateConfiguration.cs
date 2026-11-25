//added
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Infrastructure.Persistence.Configurations;

public sealed class ResumeTemplateConfiguration : IEntityTypeConfiguration<ResumeTemplate>
{
    public void Configure(EntityTypeBuilder<ResumeTemplate> builder)
    {
        builder.ToTable("ResumeTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(t => t.PreviewImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.StructureSchemaJson)
            .IsRequired();

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData( // examples of resume templates from AI
            new ResumeTemplate
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Name = "Мінімалістичний класичний",
                Slug = "minimalist-classic",
                Description = "Чистий одноколонковий шаблон, що ідеально проходить ATS-системи.", 
                Category = "IT & Tech",
                PreviewImageUrl = "/images/templates/minimalist-classic.png",
                StructureSchemaJson = "{\"columns\":1,\"theme\":\"light\"}",
                IsPremium = false,
                IsActive = true,
                CreatedAt = seedDate
            },
            new ResumeTemplate
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Name = "Сучасний двоколонковий",
                Slug = "modern-two-column",
                Description = "Акцентний шаблон із бічною панеллю для навичок та контактів.", 
                Category = "Design & Creative",
                PreviewImageUrl = "/images/templates/modern-two-column.png",
                StructureSchemaJson = "{\"columns\":2,\"theme\":\"accent-blue\"}",
                IsPremium = true,
                IsActive = true,
                CreatedAt = seedDate
            }
        );
    }
}
