using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResumeMaker.Application.DTOs.Request
{
    public class UpdateGuestResumeRequest
    {
        [Required(ErrorMessage = "Поле content є обов'язковим.")]
        public GuestResumeContentDto Content { get; set; } = null!;
    }

    public class GuestResumeContentDto
    {
        [Required(ErrorMessage = "Персональні дані є обов'язковими.")]
        public GuestPersonalInfoDto PersonalInfo { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "Максимальна кількість записів про досвід — 20.")]
        public List<GuestExperienceItemDto> Experience { get; set; } = new();

        [MaxLength(15, ErrorMessage = "Максимальна кількість записів про освіту — 15.")]
        public List<GuestEducationItemDto> Education { get; set; } = new();
    }

    public class GuestPersonalInfoDto
    {
        [Required(ErrorMessage = "ПІБ є обов'язковим.")]
        [MaxLength(150, ErrorMessage = "ПІБ не може перевищувати 150 символів.")]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? JobTitle { get; set; }

        [EmailAddress(ErrorMessage = "Некоректний формат email.")]
        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(32)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? Location { get; set; }

        [MaxLength(2000)]
        public string? Summary { get; set; }
    }

    public class GuestExperienceItemDto
    {
        [Required]
        [MaxLength(150)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string Position { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? StartDate { get; set; }

        [MaxLength(50)]
        public string? EndDate { get; set; }

        public bool IsCurrent { get; set; }

        [MaxLength(3000)]
        public string? Description { get; set; }
    }

    public class GuestEducationItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Institution { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Degree { get; set; }

        [MaxLength(150)]
        public string? FieldOfStudy { get; set; }

        [MaxLength(50)]
        public string? StartDate { get; set; }

        [MaxLength(50)]
        public string? EndDate { get; set; }
    }
}