using BBB_ApplicationDashboard.Application.DTOs.Audit;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces
{
    public interface IAuditService
    {
        Task DeleteActivityEvent(Guid id);
        Task DeleteAllActivityEvents();
        Task<ActivityEvent?> GetActivityEventById(Guid id);
        Task LogActivityEvent(ActivityEvent activityEvent);
        Task<PaginatedResponse<SimpleAuditResponse>> GetAllFilteredActivityEvents(
            AuditPaginationRequest request
        );
        Task<List<string>> GetActions();
        Task<List<string>> GetUsers();
        Task<List<string>> GetEntities();
        Task<List<string?>> GetStatuses();
        Task<List<string?>> GetUserVersions();
    }
}
