using MongoDB.Bson.Serialization.Attributes;

namespace BBB_ApplicationDashboard.Domain.Entities;

[BsonIgnoreExtraElements]
public class TOB
{
    public string CbbbId { get; set; } = null!;

    public string Name { get; set; } = null!;
}
