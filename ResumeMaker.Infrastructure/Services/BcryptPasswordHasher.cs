using ResumeMaker.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Services
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password) =>
            BCrypt.Net.BCrypt.EnhancedHashPassword(password);

        public bool VerifyPassword(string password, string passwordHash) =>
            BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
