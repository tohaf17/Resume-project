using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Request
{
    public record VerifyTelegramAuthRequest(Guid RequestId, string Code);
}
