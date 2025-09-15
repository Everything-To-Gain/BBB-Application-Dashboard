using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class TOB
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonElement("tob")]
    public string Tob { get; set; } = null!;
}
