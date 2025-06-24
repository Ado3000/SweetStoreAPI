namespace SweetStoreAPI.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProductsCollectionName { get; set; } = "Products";
    public string CustomersCollectionName { get; set; } = "Customers";
    public string OrdersCollectionName { get; set; } = "Orders";
    public string ShoppingCartsCollectionName { get; set; } = "ShoppingCarts";
}
