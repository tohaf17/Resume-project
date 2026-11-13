using ResumeMaker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(User user);
        (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken();
    }
}
