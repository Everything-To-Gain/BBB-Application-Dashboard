using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BBB_ApplicationDashboard.Infrastructure.Data.Repositories;

public class MongoN8NAuditLogsRepository : IMongoN8NAuditLogsRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public MongoN8NAuditLogsRepository(ISecretService secretService)
    {
        var client = new MongoClient(
            secretService.GetSecret(ProjectSecrets.DumpMongoDBConnection, Folders.ConnectionStrings)
        );
        var db = client.GetDatabase("Webhooks");
        _collection = db.GetCollection<BsonDocument>("audit");
    }

    public async Task InsertAsync(Dictionary<string, object> payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        // Add createdAt and filled fields to the payload
        payload["createdAt"] = DateTime.UtcNow;
        // Convert via JSON so System.Text.Json.JsonElement and nested structures are handled
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var bsonDocument = BsonDocument.Parse(json);
        await _collection.InsertOneAsync(bsonDocument);
    }

    public async Task<List<Dictionary<string, object>>> GetAllAsync()
    {
        var documents = await _collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();
        var results = new List<Dictionary<string, object>>(documents.Count);
        foreach (var doc in documents)
        {
            var dict = new Dictionary<string, object>();
            foreach (var element in doc.Elements)
            {
                var value = BsonTypeMapper.MapToDotNetValue(element.Value);
                dict[element.Name] = value;
            }

            results.Add(dict);
        }

        return results;
    }
}