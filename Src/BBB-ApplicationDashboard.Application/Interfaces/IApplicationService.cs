using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IApplicationService
{
    Task<AccreditationResponse> CreateApplicationAsync(SubmittedDataRequest request);
    Task UpdateApplicationAsync(ApplicationInfo applicationInfo);
}
