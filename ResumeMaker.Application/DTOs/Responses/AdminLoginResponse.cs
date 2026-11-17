using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses
{
    public record AdminLoginResponse(
    string AccessToken,
    string RefreshToken,
    AdminUserDto User);
}
