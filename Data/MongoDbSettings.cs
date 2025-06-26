namespace SweetStoreAPI.Data;

public record MongoDbSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string ProductsCollectionName { get; init; } = "Products";
    public string CustomersCollectionName { get; init; } = "Customers";
    public string OrdersCollectionName { get; init; } = "Orders";
    public string ShoppingCartsCollectionName { get; init; } = "ShoppingCarts";
}
