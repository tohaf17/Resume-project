using ResumeMaker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByTelegramIdAsync(long telegramUserId, CancellationToken token = default);
        Task AddUserAsync(User user, CancellationToken token = default);
        Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken token = default);
        Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash, CancellationToken token = default);
        Task<bool> TryRevokeRefreshTokenAsync(string tokenHash, DateTime revokedAt, CancellationToken token = default);
        Task SaveChangesAsync(CancellationToken token = default);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken token = default);

    }
}
