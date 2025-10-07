using BBB_ApplicationDashboard.Application;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;

namespace BBB_ApplicationDashboard.Infrastructure;

public class DumpService(IMongoDumpRepository repository, ApplicationDbContext applicationDbContext)
    : IDumpService
{
    public async Task DumpIntoMongo(Dictionary<string, object> payload, string token)
    {
        var session =
            applicationDbContext.Sessions.FirstOrDefault(session => session.Token == token)
            ?? throw new NotFoundException(
                "❌ Token not found and this is impossible, authentication sucks"
            );

        payload["source"] = session.SessionSource.ToString();
        await repository.InsertAsync(payload);
    }

    public async Task<IEnumerable<Dictionary<string, object>>> GetAllAsync()
    {
        return await repository.GetAllAsync();
    }
}
