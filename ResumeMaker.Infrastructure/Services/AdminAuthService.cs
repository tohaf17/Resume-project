using ResumeMaker.Application.DTOs;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Services
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public AdminAuthService(
            IUserRepository repository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtGenerator)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<(AdminLoginResponse? Response, string? Error)> LoginAsync(
            AdminLoginRequest request,
            CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return (null, "Email та пароль є обов'язковими.");

            var user = await _repository.GetUserByEmailAsync(request.Email, token);

            if (user is null ||
                string.IsNullOrWhiteSpace(user.PasswordHash) ||
                !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return (null, "Невірний email або пароль.");
            }

            if (user.Role != UserRole.Admin)
                return (null, "Доступ заборонено. Недостатньо прав.");

            var accessToken = _jwtGenerator.GenerateAccessToken(user);
            var (rawRefreshToken, refreshTokenHash, refreshExpiresAt) = _jwtGenerator.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                ExpiresAt = refreshExpiresAt,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddRefreshTokenAsync(refreshTokenEntity, token);
            await _repository.SaveChangesAsync(token);

            var response = new AdminLoginResponse(
                AccessToken: accessToken,
                RefreshToken: rawRefreshToken,
                User: new AdminUserDto(user.Id, user.Email!, user.Role.ToString()));

            return (response, null);
        }
    }
}
