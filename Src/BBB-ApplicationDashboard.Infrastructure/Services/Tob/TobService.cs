using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Tob;

public class TobService(IMongoDatabase database) : ITobService
{
    public async Task<List<TOB>> GetTOBs(string? searchTerm)
    {
        var col = database.GetCollection<BsonDocument>("tobs");
        if (searchTerm is null)
            return await col.Aggregate()
                .Project<TOB>(new BsonDocument { { "_id", "$_id" }, { "tob", "$properties.tob" } })
                .Limit(5)
                .ToListAsync();

        //! smart search implementation
        var regex = new BsonRegularExpression(searchTerm, "i");
        var filter = Builders<BsonDocument>.Filter.Regex("properties.tob", regex);
        var result = await col.Find(filter)
            .Project<TOB>(new BsonDocument { { "_id", "$_id" }, { "tob", "$properties.tob" } })
            .Limit(5)
            .ToListAsync();
        return result;
    }
}
