namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IN8NAuditService
{
    Task Add(Dictionary<string, object> payload);
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync();
}