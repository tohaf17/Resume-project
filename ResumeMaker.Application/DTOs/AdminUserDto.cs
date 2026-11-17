using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs
{
    public record AdminUserDto(Guid Id, string Email, string Role);
}
