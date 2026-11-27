using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Infrastructure.Persistence.Configurations
{
    public class GuestResumeDraftConfiguration : IEntityTypeConfiguration<GuestResumeDraft>
    {
        public void Configure(EntityTypeBuilder<GuestResumeDraft> builder)
        {
            builder.ToTable("GuestResumeDrafts");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.GuestSessionId)
                .IsRequired();

            builder.HasIndex(d => d.GuestSessionId);

            builder.Property(d => d.ContentJson)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired();

            builder.Property(d => d.ExpiresAt)
                .IsRequired();

            builder.HasOne(d => d.Template)
                .WithMany()
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}