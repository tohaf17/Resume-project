using System;
using System.ComponentModel.DataAnnotations;

namespace ResumeMaker.Application.DTOs.Request
{
    public class CreateGuestResumeRequest
    {
        [Required(ErrorMessage = "TemplateId є обов'язковим.")]
        public Guid TemplateId { get; set; }
    }
}