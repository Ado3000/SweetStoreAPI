namespace SweetStoreAPI.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductCategory Category { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public int StockQuantity { get; set; }
}

public enum ProductCategory
{
    Cookies = 1,
    IceCream = 2,
    Candy = 3
}
