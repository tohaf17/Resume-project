using ResumeMaker.Application.DTOs.Request;
using ResumeMaker.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResumeMaker.Application.Interfaces.Services
{
    public interface IAdminAuthService
    {
        Task<(AdminLoginResponse? Response, string? Error)> LoginAsync(
            AdminLoginRequest request,
            CancellationToken token = default);
    }
}
