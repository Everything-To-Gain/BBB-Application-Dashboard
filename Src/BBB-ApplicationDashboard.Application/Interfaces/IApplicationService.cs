using BBB_ApplicationDashboard.Application.DTOs;
using BBB_ApplicationDashboard.Application.DTOs.Application;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IApplicationService
{
    Task<AccreditationResponse> CreateApplicationAsync(SubmittedDataRequest request);
    Task UpdateApplicationAsync(ApplicationInfo applicationInfo);
    Task<PaginatedResponse<InternalApplicationResponse>> GetInternalData(
        InternalPaginationRequest request
    );
    Task<PaginatedResponse<ExternalApplicationResponse>> GetExternalData(
        ExternalPaginationRequest request,
        Source source
    );
    Task<PaginatedResponse<ExternalApplicationResponse>> GetExternalDataForAdmins(
        AdminExternalPaginationRequest request
    );
    Task<bool> UpdateApplicationStatus(
        UpdateApplicationStatusRequest updateApplicationStatusRequest
    );

    Task<Accreditation> GetApplicationById(Guid id);
}
