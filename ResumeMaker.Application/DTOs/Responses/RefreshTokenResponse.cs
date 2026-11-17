using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.DTOs.Responses
{
    public record RefreshTokenResponse(string AccessToken, string RefreshToken);
}
