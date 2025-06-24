namespace SweetStoreAPI.Models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public decimal SubTotal => OrderItems.Sum(item => item.TotalPrice);
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount => SubTotal - DiscountAmount;
    public OrderStatus Status { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Cancelled = 4
}
