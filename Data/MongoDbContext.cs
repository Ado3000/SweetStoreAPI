using MongoDB.Driver;
using SweetStoreAPI.Models;
using Microsoft.Extensions.Options;
using System;

namespace SweetStoreAPI.Data;

/// <summary>
/// MongoDB database context that follows Entity Framework conventions
/// </summary>
public class MongoDbContext : IDisposable
{
    private readonly IMongoDatabase _database;
    private readonly MongoClient _client;
    private bool _disposed = false;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        if (settings?.Value == null)
            throw new ArgumentNullException(nameof(settings), "MongoDB settings cannot be null");

        if (string.IsNullOrEmpty(settings.Value.ConnectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(settings));

        if (string.IsNullOrEmpty(settings.Value.DatabaseName))
            throw new ArgumentException("Database name cannot be null or empty", nameof(settings));

        _client = new MongoClient(settings.Value.ConnectionString);
        _database = _client.GetDatabase(settings.Value.DatabaseName);

        // Initialize collections and apply any configurations
        OnModelCreating();
    }

    /// <summary>
    /// Gets the Products collection
    /// </summary>
    public IMongoCollection<Product> Products { get; private set; } = null!;

    /// <summary>
    /// Gets the Customers collection
    /// </summary>
    public IMongoCollection<Customer> Customers { get; private set; } = null!;

    /// <summary>
    /// Gets the Orders collection
    /// </summary>
    public IMongoCollection<Order> Orders { get; private set; } = null!;

    /// <summary>
    /// Gets the ShoppingCarts collection
    /// </summary>
    public IMongoCollection<ShoppingCart> ShoppingCarts { get; private set; } = null!;

    /// <summary>
    /// Configures the MongoDB collections and indexes (similar to EF's OnModelCreating)
    /// </summary>
    protected virtual void OnModelCreating()
    {
        // Initialize collections
        Products = _database.GetCollection<Product>("Products");
        Customers = _database.GetCollection<Customer>("Customers");
        Orders = _database.GetCollection<Order>("Orders");
        ShoppingCarts = _database.GetCollection<ShoppingCart>("ShoppingCarts");

        // Configure indexes and constraints
        ConfigureProductsCollection();
        ConfigureCustomersCollection();
        ConfigureOrdersCollection();
        ConfigureShoppingCartsCollection();
    }

    /// <summary>
    /// Configures the Products collection indexes and constraints
    /// </summary>
    private void ConfigureProductsCollection()
    {
        // Example: Create indexes for better performance
        var productIndexKeys = Builders<Product>.IndexKeys.Ascending(p => p.Name);
        var productIndexModel = new CreateIndexModel<Product>(productIndexKeys);
        // Products.Indexes.CreateOneAsync(productIndexModel); // Uncomment to create index
    }

    /// <summary>
    /// Configures the Customers collection indexes and constraints
    /// </summary>
    private void ConfigureCustomersCollection()
    {
        // Example: Create unique index on email
        var customerIndexKeys = Builders<Customer>.IndexKeys.Ascending(c => c.Email);
        var customerIndexOptions = new CreateIndexOptions { Unique = true };
        var customerIndexModel = new CreateIndexModel<Customer>(customerIndexKeys, customerIndexOptions);
        // Customers.Indexes.CreateOneAsync(customerIndexModel); // Uncomment to create index
    }

    /// <summary>
    /// Configures the Orders collection indexes and constraints
    /// </summary>
    private void ConfigureOrdersCollection()
    {
        // Example: Create compound index on CustomerId and OrderDate
        var orderIndexKeys = Builders<Order>.IndexKeys
            .Ascending(o => o.CustomerId)
            .Descending(o => o.OrderDate);
        var orderIndexModel = new CreateIndexModel<Order>(orderIndexKeys);
        // Orders.Indexes.CreateOneAsync(orderIndexModel); // Uncomment to create index
    }

    /// <summary>
    /// Configures the ShoppingCarts collection indexes and constraints
    /// </summary>
    private void ConfigureShoppingCartsCollection()
    {
        // Example: Create index on CustomerId
        var cartIndexKeys = Builders<ShoppingCart>.IndexKeys.Ascending(sc => sc.CustomerId);
        var cartIndexModel = new CreateIndexModel<ShoppingCart>(cartIndexKeys);
        // ShoppingCarts.Indexes.CreateOneAsync(cartIndexModel); // Uncomment to create index
    }

    /// <summary>
    /// Saves changes to the database (placeholder for EF-like behavior)
    /// Note: MongoDB doesn't have transactions like EF, but this provides a consistent API
    /// </summary>
    public virtual async Task<int> SaveChangesAsync()
    {
        // In MongoDB, changes are typically saved immediately
        // This method is here for EF compatibility
        // You could implement transaction logic here if needed
        return await Task.FromResult(0);
    }

    /// <summary>
    /// Saves changes to the database synchronously
    /// </summary>
    public virtual int SaveChanges()
    {
        // In MongoDB, changes are typically saved immediately
        // This method is here for EF compatibility
        return 0;
    }

    /// <summary>
    /// Dispose pattern implementation
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose method
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _client?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~MongoDbContext()
    {
        Dispose(false);
    }
}
