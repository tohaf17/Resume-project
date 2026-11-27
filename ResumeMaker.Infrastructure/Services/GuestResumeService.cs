using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using ResumeMaker.Application.Interfaces.Services;
using ResumeMaker.Domain.Entities;
using ResumeMaker.Infrastructure.Persistence;

namespace ResumeMaker.Infrastructure.Services
{
    public class GuestResumeService : IGuestResumeService
    {
        private readonly AppDbContext _dbContext;
        private static readonly TimeSpan DraftLifetime = TimeSpan.FromDays(7);
        private const int MaxContentBytes = 64 * 1024; // Максимум 64 KB на JSON чернетки

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GuestResumeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ServiceResult<(CreateGuestResumeResponse Response, Guid GuestSessionId)>> CreateDraftAsync(
            CreateGuestResumeRequest request,
            Guid? existingGuestSessionId,
            CancellationToken cancellationToken = default)
        {
            var template = await _dbContext.ResumeTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.TemplateId && t.IsActive, cancellationToken);

            if (template is null)
            {
                return new ServiceResult<(CreateGuestResumeResponse, Guid)>(
                    default,
                    "Вказаний шаблон не знайдено або він недоступний.",
                    GuestResumeErrorType.BadRequest);
            }

            if (template.IsPremium)
            {
                return new ServiceResult<(CreateGuestResumeResponse, Guid)>(
                    default,
                    "Обраний шаблон доступний лише для авторизованих користувачів із підпискою.",
                    GuestResumeErrorType.BadRequest);
            }

            var guestSessionId = existingGuestSessionId ?? Guid.NewGuid();
            var now = DateTime.UtcNow;
            var expiresAt = now.Add(DraftLifetime);

            var initialContent = new GuestResumeContentDto
            {
                PersonalInfo = new GuestPersonalInfoDto()
            };

            var draft = new GuestResumeDraft
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                GuestSessionId = guestSessionId,
                ContentJson = JsonSerializer.Serialize(initialContent, JsonOptions),
                CreatedAt = now,
                UpdatedAt = now,
                ExpiresAt = expiresAt
            };

            _dbContext.GuestResumeDrafts.Add(draft);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new CreateGuestResumeResponse
            {
                Id = draft.Id,
                TemplateId = draft.TemplateId,
                ExpiresAt = DateTime.SpecifyKind(draft.ExpiresAt, DateTimeKind.Utc)
            };

            return new ServiceResult<(CreateGuestResumeResponse, Guid)>((response, guestSessionId), null);
        }

        public async Task<ServiceResult<GuestResumeDraftResponse>> UpdateDraftAsync(
            Guid draftId,
            Guid guestSessionId,
            UpdateGuestResumeRequest request,
            CancellationToken cancellationToken = default)
        {
            var serializedContent = JsonSerializer.Serialize(request.Content, JsonOptions);
            if (Encoding.UTF8.GetByteCount(serializedContent) > MaxContentBytes)
            {
                return new ServiceResult<GuestResumeDraftResponse>(
                    null,
                    $"Розмір даних чернетки перевищує максимально допустимий ліміт ({MaxContentBytes / 1024} KB).",
                    GuestResumeErrorType.BadRequest);
            }

            var draft = await _dbContext.GuestResumeDrafts
                .FirstOrDefaultAsync(d => d.Id == draftId && d.GuestSessionId == guestSessionId, cancellationToken);

            if (draft is null)
            {
                return new ServiceResult<GuestResumeDraftResponse>(
                    null,
                    "Чернетку не знайдено.",
                    GuestResumeErrorType.NotFound);
            }

            var now = DateTime.UtcNow;
            if (draft.IsExpired(now))
            {
                return new ServiceResult<GuestResumeDraftResponse>(
                    null,
                    "Термін дії гостьової чернетки закінчився. Редагування недоступне.",
                    GuestResumeErrorType.Gone);
            }

            draft.ContentJson = serializedContent;
            draft.UpdatedAt = now;

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = new GuestResumeDraftResponse
            {
                Id = draft.Id,
                TemplateId = draft.TemplateId,
                Content = request.Content,
                UpdatedAt = DateTime.SpecifyKind(draft.UpdatedAt, DateTimeKind.Utc),
                ExpiresAt = DateTime.SpecifyKind(draft.ExpiresAt, DateTimeKind.Utc)
            };

            return new ServiceResult<GuestResumeDraftResponse>(response, null);
        }
    }
}