//added
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Infrastructure.Persistence.Configurations;

public sealed class StaticPageConfiguration : IEntityTypeConfiguration<StaticPage>
{
    public void Configure(EntityTypeBuilder<StaticPage> builder)
    {
        builder.ToTable("StaticPages");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Content)
            .IsRequired();

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            new StaticPage
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Slug = "about",
                Title = "Про сервіс Resume Maker",
                Content = "Resume Maker — сучасний онлайн-конструктор для швидкого створення професійних резюме.", // text was writen by AI
                IsPublished = true,
                UpdatedAt = seedDate
            },
            new StaticPage
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Slug = "privacy-policy",
                Title = "Політика конфіденційності",
                Content = "Ми поважаємо вашу приватність та захищаємо персональні дані відповідно до чинного законодавства.", // text was writen by AI
                IsPublished = true,
                UpdatedAt = seedDate
            },
            new StaticPage
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Slug = "terms-of-service",
                Title = "Умови використання",
                Content = "Використовуючи платформу Resume Maker, ви погоджуєтесь із правилами надання сервісу.", // text was writen by AI
                IsPublished = true,
                UpdatedAt = seedDate
            }
        );
    }
}
