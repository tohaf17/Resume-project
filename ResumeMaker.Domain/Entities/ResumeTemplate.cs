//added
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Domain.Entities;

public class ResumeTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string PreviewImageUrl { get; set; } = string.Empty;
    public string StructureSchemaJson { get; set; } = "{}";
    public bool IsPremium { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
