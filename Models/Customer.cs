using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SweetStoreAPI.Models;

public class Customer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("customerId")]
    public int CustomerId { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
    
    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [BsonElement("type")]
    public CustomerType Type { get; set; }
    
    [BsonElement("discountPercentage")]
    public decimal DiscountPercentage { get; set; }
}

public enum CustomerType
{
    Regular = 1,
    Premium = 2,
    VIP = 3
}
