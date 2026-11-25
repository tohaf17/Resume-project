//added
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Domain.Entities;

public class StaticPage
{
    public Guid Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = true;
    public DateTime UpdatedAt { get; set; }
}
