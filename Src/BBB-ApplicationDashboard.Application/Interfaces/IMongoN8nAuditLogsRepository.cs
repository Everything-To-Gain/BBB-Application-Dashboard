namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IMongoN8NAuditLogsRepository
{
    Task InsertAsync(Dictionary<string, object> payload);
    Task<List<Dictionary<string, object>>> GetAllAsync();
}