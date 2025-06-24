using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SweetStoreAPI.Models;

public class ShoppingCart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("customerId")]
    public int CustomerId { get; set; }
    
    [BsonElement("items")]
    public List<CartItem> Items { get; set; } = new();
    
    [BsonIgnore]
    public decimal SubTotal => Items.Sum(item => item.TotalPrice);
}

public class CartItem
{
    [BsonElement("productId")]
    public int ProductId { get; set; }
    
    [BsonElement("product")]
    public Product Product { get; set; } = new();
    
    [BsonElement("quantity")]
    public int Quantity { get; set; }
    
    [BsonElement("unitPrice")]
    public decimal UnitPrice { get; set; }
    
    [BsonIgnore]
    public decimal TotalPrice => Quantity * UnitPrice;
}
