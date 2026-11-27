using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Infrastructure.Persistence.Configurations
{
    public class ResumeTemplateConfiguration : IEntityTypeConfiguration<ResumeTemplate>
    {
        public void Configure(EntityTypeBuilder<ResumeTemplate> builder)
        {
            builder.ToTable("ResumeTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(t => t.IsPremium)
                .IsRequired();

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();
        }
    }
}