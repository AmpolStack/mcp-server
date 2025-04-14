using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Services.Models;

public class Client
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }
    
    [BsonElement("firstName")]
    public string? FirstName { get; set; }
    
    [BsonElement("lastName")]
    public string? LastName { get; set; }
    
    [BsonElement("email")]
    public string? Email { get; set; }
    
    [BsonElement("phone")]
    public string? Phone { get; set; }
    
    [BsonElement("address")]
    public Address? Address { get; set; }
    
    [BsonElement("isActive")]
    public bool IsActive { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

}