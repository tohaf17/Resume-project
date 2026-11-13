using Microsoft.EntityFrameworkCore;
using ResumeMaker.Application.Interfaces.Repositories;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> GetUserByTelegramIdAsync(long telegramUserId, CancellationToken token = default)
        {
            return await _dbContext.Users
                .AsQueryable()
                .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId, token);
        }

        public async Task AddUserAsync(User user, CancellationToken token = default)
        {
            await _dbContext.Users.AddAsync(user, token);
        }

        public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken token = default)
        {
            await _dbContext.RefreshTokens.AddAsync(refreshToken, token);
        }

        public async Task SaveChangesAsync(CancellationToken token = default)
        {
            await _dbContext.SaveChangesAsync(token);
        }
    }
}
