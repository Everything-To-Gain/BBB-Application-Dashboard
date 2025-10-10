using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using BBB_ApplicationDashboard.Application;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace BBB_ApplicationDashboard.Infrastructure;

public class MongoDumpRepository : IMongoDumpRepository
{
    private readonly IMongoCollection<BsonDocument> collection;

    public MongoDumpRepository(ISecretService secretService)
    {
        var client = new MongoClient(
            secretService.GetSecret(ProjectSecrets.DumpMongoDBConnection, Folders.ConnectionStrings)
        );
        var db = client.GetDatabase("default");
        collection = db.GetCollection<BsonDocument>("dump");
    }

    public async Task InsertAsync(Dictionary<string, object> payload)
    {
        if (payload == null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        // Add createdAt and filled fields to the payload
        payload["createdAt"] = DateTime.UtcNow;
        payload["processed"] = false;

        // Convert via JSON so System.Text.Json.JsonElement and nested structures are handled
        var json = JsonSerializer.Serialize(payload);
        var bsonDocument = BsonDocument.Parse(json);

        await collection.InsertOneAsync(bsonDocument);
    }

    public async Task<List<Dictionary<string, object>>> GetAllAsync()
    {
        var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();

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
