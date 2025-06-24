using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SweetStoreAPI.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("productId")]
    public int ProductId { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;
    
    [BsonElement("price")]
    public decimal Price { get; set; }
    
    [BsonElement("category")]
    public ProductCategory Category { get; set; }
    
    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
    
    [BsonElement("isAvailable")]
    public bool IsAvailable { get; set; } = true;
    
    [BsonElement("stockQuantity")]
    public int StockQuantity { get; set; }
}

public enum ProductCategory
{
    Cookies = 1,
    IceCream = 2,
    Candy = 3
}
