using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SweetStoreAPI.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("orderId")]
    public int OrderId { get; set; }
    
    [BsonElement("customerId")]
    public int CustomerId { get; set; }
    
    [BsonElement("customer")]
    public Customer Customer { get; set; } = new();
    
    [BsonElement("orderDate")]
    public DateTime OrderDate { get; set; }
    
    [BsonElement("orderItems")]
    public List<OrderItem> OrderItems { get; set; } = new();
    
    [BsonIgnore]
    public decimal SubTotal => OrderItems.Sum(item => item.TotalPrice);
    
    [BsonElement("discountAmount")]
    public decimal DiscountAmount { get; set; }
    
    [BsonIgnore]
    public decimal TotalAmount => SubTotal - DiscountAmount;
    
    [BsonElement("status")]
    public OrderStatus Status { get; set; }
}

public class OrderItem
{
    [BsonElement("orderItemId")]
    public int OrderItemId { get; set; }
    
    [BsonElement("orderId")]
    public int OrderId { get; set; }
    
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

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4
}
