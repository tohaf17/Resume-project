using ResumeMaker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs
{
    public record UserDto(Guid Id, string? TelegramUsername, string Role);
    
}
