using ResumeMaker.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Infrastructure.Persistence;

namespace ResumeMaker.Infrastructure.Repositories
{
    public class TelegramAuthRepository:ITelegramAuthRepository
    {
        private readonly AppDbContext _context;
        public TelegramAuthRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(TelegramAuthRequest request, CancellationToken token = default!)
        {
            await _context.TelegramAuthRequests.AddAsync(request, token);
        }
        public async Task SaveChangesAsync(CancellationToken token = default!)
        {
            await _context.SaveChangesAsync(token);
        }

        public async Task<TelegramAuthRequest?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            return await _context.TelegramAuthRequests.FindAsync([id], token);
        }
    }
}
