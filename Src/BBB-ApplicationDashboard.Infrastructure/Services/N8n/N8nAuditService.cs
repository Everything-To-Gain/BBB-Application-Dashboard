namespace BBB_ApplicationDashboard.Infrastructure.Services.N8n;

using BBB_ApplicationDashboard.Application.Interfaces;
using global::BBB_ApplicationDashboard.Application.Interfaces;
using MongoDB.Driver;

public class N8NAuditService(IMongoN8NAuditLogsRepository repo) : IN8NAuditService
{
    public Task Add(Dictionary<string, object> payload)
    {
        return repo.InsertAsync(payload);
    }

    public async Task<IEnumerable<Dictionary<string, object>>> GetAllAsync()
    {
        return await repo.GetAllAsync();
    }
}