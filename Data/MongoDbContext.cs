using MongoDB.Driver;
using SweetStoreAPI.Models;
using Microsoft.Extensions.Options;

namespace SweetStoreAPI.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<Product> Products => 
        _database.GetCollection<Product>("Products");

    public IMongoCollection<Customer> Customers => 
        _database.GetCollection<Customer>("Customers");

    public IMongoCollection<Order> Orders => 
        _database.GetCollection<Order>("Orders");

    public IMongoCollection<ShoppingCart> ShoppingCarts => 
        _database.GetCollection<ShoppingCart>("ShoppingCarts");
}
