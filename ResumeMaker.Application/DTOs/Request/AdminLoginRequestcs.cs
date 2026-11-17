using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Request
{
    public record AdminLoginRequest(string? Email, string? Password);
}
