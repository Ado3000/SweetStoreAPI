namespace SweetStoreAPI.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CustomerType Type { get; set; }
    public decimal DiscountPercentage { get; set; }
}

public enum CustomerType
{
    Regular = 1,
    Premium = 2,
    VIP = 3
}
