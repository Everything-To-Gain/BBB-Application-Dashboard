using BBB_ApplicationDashboard.Application;
using MongoDB.Bson;

namespace BBB_ApplicationDashboard.Infrastructure;

public class DumpService(IMongoDumpRepository repository) : IDumpService
{
    public async Task DumpIntoMongo(Dictionary<string, object> payload)
    {
        await repository.InsertAsync(payload);
    }

    public async Task<IEnumerable<Dictionary<string, object>>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }
}
