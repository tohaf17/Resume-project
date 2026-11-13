using ResumeMaker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Application.DTOs;

namespace ResumeMaker.Application.DTOs.Responses
{
    public record VerifyTelegramAuthResponse
    (
        string AccessToken,
        string RefreshToken,
        bool IsNewUser,
        UserDto User
    );
}
