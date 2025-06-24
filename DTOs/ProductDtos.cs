namespace SweetStoreAPI.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }
}

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public int CustomerId { get; set; }
}

public class CartDto
{
    public int CustomerId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
