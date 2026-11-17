using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ResumeMaker.Application.DTOs;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Domain.Enums;
using ResumeMaker.Application.Options;
using ResumeMaker.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace ResumeMaker.Infrastructure.Services
{
    public class TelegramAuthService : ITelegramAuthService
    {
        private readonly ITelegramAuthRepository _telegramRepository;
        private readonly IUserRepository _userRepository;
        private readonly TelegramBotOptions _botOptions;
        private readonly IJwtTokenGenerator _jwtGenerator;
        public TelegramAuthService(ITelegramAuthRepository telegramAuthRepository, IUserRepository userRepository, IOptions<TelegramBotOptions> botOptions, IJwtTokenGenerator jwtGenerator)
        {
            _telegramRepository = telegramAuthRepository;
            _userRepository = userRepository;
            _botOptions = botOptions.Value;
            _jwtGenerator = jwtGenerator;
        }
        public async Task<TelegramAuthResponse> InitAsync(CancellationToken token = default!)
        {
            var request = new TelegramAuthRequest();

            request.Status = StatusRequest.Pending;
            await _telegramRepository.AddAsync(request, token);
            await _telegramRepository.SaveChangesAsync(token);

            var deepLink = $"https://t.me/{_botOptions.Username}?start={request.Id}";
            var expiresInSeconds = (int)(request.ExpiresAt - request.CreatedAt).TotalSeconds;
            return new TelegramAuthResponse(
                RequestId: request.Id,
                TelegramDeepLink: deepLink,
                ExpiresInSeconds: expiresInSeconds);
        }
        public async Task<string?> IssueCodeAsync(Guid requestId, string? telegramUsername, long chatId, CancellationToken token = default)
        {
            var authRequest = await _telegramRepository.GetByIdAsync(requestId, token);

            if (authRequest is null ||
                authRequest.Status != StatusRequest.Pending ||
                authRequest.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            var otpCode = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var salt = RandomNumberGenerator.GetBytes(16);
            var codeHash = CreateOtpHash(otpCode, salt);

            authRequest.TelegramChatId = chatId;
            authRequest.TelegramUsername = telegramUsername;
            authRequest.CodeHash = codeHash;
            authRequest.Status = StatusRequest.CodeIssued;

            await _telegramRepository.SaveChangesAsync(token);

            return otpCode;
        }

        public async Task<TelegramAuthStatusResponse?> GetStatusAsync(Guid requestId, CancellationToken token = default)
        {
            var authRequest = await _telegramRepository.GetByIdAsync(requestId, token);

            if (authRequest is null)
            {
                return null;
            }
            if (authRequest.ExpiresAt <= DateTime.UtcNow &&
                authRequest.Status is StatusRequest.Pending or StatusRequest.CodeIssued)
            {
                authRequest.Status = StatusRequest.Expired;
                await _telegramRepository.SaveChangesAsync(token);
            }

            return new TelegramAuthStatusResponse(authRequest.Status.ToString());
        }

        public async Task<(VerifyTelegramAuthResponse? response, string? error)> VerifyAuthAsync(
    VerifyTelegramAuthRequest request,
    CancellationToken token = default)
        {
            var authRequest = await _telegramRepository.GetByIdAsync(request.RequestId, token);

            if (authRequest is null)
                return (null, "Заявку авторизації не знайдено.");

            if (authRequest.Status != StatusRequest.CodeIssued)
                return (null, "Заявка має недійсний статус для підтвердження.");

            if (authRequest.ExpiresAt <= DateTime.UtcNow)
            {
                authRequest.Status = StatusRequest.Expired;
                await _telegramRepository.SaveChangesAsync(token);
                return (null, "Термін дії коду вийшов.");
            }

            if (string.IsNullOrWhiteSpace(request.Code) || request.Code.Length != 6 ||
                !request.Code.All(char.IsDigit))
                return (null, "Код має містити рівно 6 цифр.");

            var isCodeValid = VerifyOtpHash(request.Code, authRequest.CodeHash);

            if (!isCodeValid)
            {
                authRequest.Attempts += 1;

                if (authRequest.Attempts >= 5)
                    authRequest.Status = StatusRequest.Expired;

                await _telegramRepository.SaveChangesAsync(token);

                var message = authRequest.Status == StatusRequest.Expired
                    ? "Перевищено кількість спроб. Заявка анульована."
                    : $"Невірний код підтвердження. Залишилось спроб: {5 - authRequest.Attempts}.";

                return (null, message);
            }

            authRequest.Status = StatusRequest.Confirmed;

            if (authRequest.TelegramChatId is null)
                return (null, "Telegram-користувача для заявки не знайдено.");

            var telegramUserId = authRequest.TelegramChatId.Value;
            var user = await _userRepository.GetUserByTelegramIdAsync(telegramUserId, token);
            var isNewUser = false;

            if (user is null)
            {
                isNewUser = true;
                user = new User
                {
                    Id = Guid.NewGuid(),
                    TelegramUserId = telegramUserId,
                    TelegramUsername = authRequest.TelegramUsername,
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepository.AddUserAsync(user, token);
            }
            else if (!string.IsNullOrWhiteSpace(authRequest.TelegramUsername) &&
                     user.TelegramUsername != authRequest.TelegramUsername)
            {
                user.TelegramUsername = authRequest.TelegramUsername;
            }

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

            await _userRepository.AddRefreshTokenAsync(refreshTokenEntity, token);
            await _userRepository.SaveChangesAsync(token);

            var response = new VerifyTelegramAuthResponse(
                AccessToken: accessToken,
                RefreshToken: rawRefreshToken,
                IsNewUser: isNewUser,
                User: new UserDto(user.Id, user.TelegramUsername, user.Role.ToString()));

            return (response, null);
        }

        public async Task<(RefreshTokenResponse? Response, string? Error)> RefreshTokenAsync(
    RefreshTokenRequest request,
    CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return (null, "Refresh token не може бути порожнім.");

            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken.Trim())));

            var existingToken = await _userRepository.GetRefreshTokenByHashAsync(tokenHash, token);

            if (existingToken is null ||
                existingToken.ExpiresAt <= DateTime.UtcNow ||
                existingToken.RevokedAt is not null)
            {
                return (null, "Недійсний або прострочений refresh token.");
            }

            if (existingToken.User is null)
                return (null, "Користувача для refresh token не знайдено.");

            if (!await _userRepository.TryRevokeRefreshTokenAsync(tokenHash, DateTime.UtcNow, token))
                return (null, "Недійсний або вже відкликаний refresh token.");

            var newAccessToken = _jwtGenerator.GenerateAccessToken(existingToken.User);
            var (newRawRefreshToken, newRefreshTokenHash, newRefreshExpiresAt) = _jwtGenerator.GenerateRefreshToken();

            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = existingToken.UserId,
                TokenHash = newRefreshTokenHash,
                ExpiresAt = newRefreshExpiresAt,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddRefreshTokenAsync(newRefreshTokenEntity, token);
            await _userRepository.SaveChangesAsync(token);

            return (new RefreshTokenResponse(newAccessToken, newRawRefreshToken), null);
        }

        public async Task<bool> LogoutAsync(Guid userId, string? refreshToken, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken.Trim())));
            var existingToken = await _userRepository.GetRefreshTokenByHashAsync(tokenHash, token);

            if (existingToken is null || existingToken.UserId != userId || existingToken.RevokedAt is not null)
                return false;

            return await _userRepository.TryRevokeRefreshTokenAsync(tokenHash, DateTime.UtcNow, token);
        }

        private static string CreateOtpHash(string code, byte[] salt)
        {
            var input = salt.Concat(Encoding.UTF8.GetBytes(code)).ToArray();
            var hash = SHA256.HashData(input);
            return $"{Convert.ToHexString(salt)}:{Convert.ToHexString(hash)}";
        }

        private static bool VerifyOtpHash(string code, string? storedValue)
        {
            if (string.IsNullOrWhiteSpace(storedValue))
                return false;

            var parts = storedValue.Split(':', 2);
            if (parts.Length != 2)
                return false;

            try
            {
                var salt = Convert.FromHexString(parts[0]);
                var expectedHash = Convert.FromHexString(parts[1]);
                var actualHash = SHA256.HashData(salt.Concat(Encoding.UTF8.GetBytes(code)).ToArray());
                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
