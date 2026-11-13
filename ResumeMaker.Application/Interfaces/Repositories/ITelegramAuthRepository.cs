using System;
using System.Collections.Generic;
using System.Text;
using ResumeMaker.Domain.Entities;

namespace ResumeMaker.Application.Interfaces.Repositories
{
    public interface ITelegramAuthRepository
    {
        Task AddAsync(TelegramAuthRequest request,CancellationToken token=default!);
        Task SaveChangesAsync(CancellationToken token = default!);
        Task<TelegramAuthRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
