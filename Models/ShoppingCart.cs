namespace SweetStoreAPI.Models;

public class ShoppingCart
{
    public int CustomerId { get; set; }
    public List<CartItem> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(item => item.TotalPrice);
}

public class CartItem
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
