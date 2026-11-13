using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses
{
    public record TelegramAuthResponse
    (
        Guid RequestId,
        string TelegramDeepLink,
        int ExpiresInSeconds
    );
}
