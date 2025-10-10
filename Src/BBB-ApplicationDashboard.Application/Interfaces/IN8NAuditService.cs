namespace BBB_ApplicationDashboard.Infrastructure.Services.N8n;

public interface IN8NAuditService
{
    Task Add(Dictionary<string, object> payload);
    Task<IEnumerable<Dictionary<string, object>>> GetAllAsync();
}