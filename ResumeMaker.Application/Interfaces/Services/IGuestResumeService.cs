using System;
using System.Threading;
using System.Threading.Tasks;
using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;

namespace ResumeMaker.Application.Interfaces.Services
{
    public enum GuestResumeErrorType
    {
        None,
        BadRequest,
        NotFound,
        Gone
    }

    public record ServiceResult<T>(T? Data, string? ErrorMessage, GuestResumeErrorType ErrorType = GuestResumeErrorType.None)
    {
        public bool IsSuccess => ErrorType == GuestResumeErrorType.None;
    }

    public interface IGuestResumeService
    {
        Task<ServiceResult<(CreateGuestResumeResponse Response, Guid GuestSessionId)>> CreateDraftAsync(
            CreateGuestResumeRequest request,
            Guid? existingGuestSessionId,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<GuestResumeDraftResponse>> UpdateDraftAsync(
            Guid draftId,
            Guid guestSessionId,
            UpdateGuestResumeRequest request,
            CancellationToken cancellationToken = default);
    }
}