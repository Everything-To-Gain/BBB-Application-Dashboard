using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces
{
    public interface IAuditService
    {
        Task DeleteActivityEvent(Guid id);
        Task DeleteAllActivityEvents();
        Task<List<string>> GetActions();
        Task<ActivityEvent?> GetActivityEventById(Guid id);
        Task<List<ActivityEvent>> GetActivityEvents(int page = 1, int pageSize = 10);
        Task<int> GetTotalActivityEventCount();
        Task LogActivityEvent(ActivityEvent activityEvent);
    }
}