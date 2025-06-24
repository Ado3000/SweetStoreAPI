using MongoDB.Driver;
using SweetStoreAPI.Data;
using SweetStoreAPI.Models;

namespace SweetStoreAPI.Services;

public class MongoDataService
{
    private readonly MongoDbContext _context;
    private static int _nextProductId = 1;
    private static int _nextCustomerId = 1;
    private static int _nextOrderId = 1;
    private static int _nextOrderItemId = 1;

    public MongoDataService(MongoDbContext context)
    {
        _context = context;
        InitializeCounters();
    }

    private async void InitializeCounters()
    {
        // Initialize counters based on existing data
        var lastProduct = await _context.Products
            .Find(_ => true)
            .SortByDescending(p => p.ProductId)
            .FirstOrDefaultAsync();
        if (lastProduct != null)
            _nextProductId = lastProduct.ProductId + 1;

        var lastCustomer = await _context.Customers
            .Find(_ => true)
            .SortByDescending(c => c.CustomerId)
            .FirstOrDefaultAsync();
        if (lastCustomer != null)
            _nextCustomerId = lastCustomer.CustomerId + 1;

        var lastOrder = await _context.Orders
            .Find(_ => true)
            .SortByDescending(o => o.OrderId)
            .FirstOrDefaultAsync();
        if (lastOrder != null)
            _nextOrderId = lastOrder.OrderId + 1;
    }

    // Products
    public async Task<List<Product>> GetAllProductsAsync() => 
        await _context.Products.Find(_ => true).ToListAsync();

    public async Task<List<Product>> GetProductsByCategoryAsync(ProductCategory category) => 
        await _context.Products.Find(p => p.Category == category).ToListAsync();

    public async Task<Product?> GetProductAsync(int productId) => 
        await _context.Products.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

    public async Task<Product> CreateProductAsync(Product product)
    {
        product.ProductId = _nextProductId++;
        await _context.Products.InsertOneAsync(product);
        return product;
    }

    public async Task UpdateProductAsync(Product product) => 
        await _context.Products.ReplaceOneAsync(p => p.ProductId == product.ProductId, product);

    public async Task DeleteProductAsync(int productId) => 
        await _context.Products.DeleteOneAsync(p => p.ProductId == productId);

    // Customers
    public async Task<List<Customer>> GetAllCustomersAsync() => 
        await _context.Customers.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetCustomerAsync(int customerId) => 
        await _context.Customers.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        customer.CustomerId = _nextCustomerId++;
        customer.DiscountPercentage = customer.Type switch
        {
            CustomerType.Regular => 0,
            CustomerType.Premium => 10,
            CustomerType.VIP => 20,
            _ => 0
        };
        await _context.Customers.InsertOneAsync(customer);
        return customer;
    }

    public async Task UpdateCustomerAsync(Customer customer) => 
        await _context.Customers.ReplaceOneAsync(c => c.CustomerId == customer.CustomerId, customer);

    public async Task DeleteCustomerAsync(int customerId) => 
        await _context.Customers.DeleteOneAsync(c => c.CustomerId == customerId);

    // Shopping Cart
    public async Task<ShoppingCart> GetCartAsync(int customerId)
    {
        var cart = await _context.ShoppingCarts.Find(c => c.CustomerId == customerId).FirstOrDefaultAsync();
        if (cart == null)
        {
            cart = new ShoppingCart { CustomerId = customerId };
            await _context.ShoppingCarts.InsertOneAsync(cart);
        }
        return cart;
    }

    public async Task AddToCartAsync(int customerId, int productId, int quantity)
    {
        var cart = await GetCartAsync(customerId);
        var product = await GetProductAsync(productId);
        
        if (product == null || !product.IsAvailable || product.StockQuantity < quantity)
            throw new InvalidOperationException("Product not available or insufficient stock");

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Product = product,
                Quantity = quantity,
                UnitPrice = product.Price
            });
        }

        await _context.ShoppingCarts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
    }

    public async Task RemoveFromCartAsync(int customerId, int productId)
    {
        var cart = await GetCartAsync(customerId);
        cart.Items.RemoveAll(i => i.ProductId == productId);
        await _context.ShoppingCarts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
    }

    public async Task UpdateCartItemQuantityAsync(int customerId, int productId, int quantity)
    {
        var cart = await GetCartAsync(customerId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        
        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            await _context.ShoppingCarts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
        }
    }

    public async Task ClearCartAsync(int customerId)
    {
        var cart = await GetCartAsync(customerId);
        cart.Items.Clear();
        await _context.ShoppingCarts.ReplaceOneAsync(c => c.CustomerId == customerId, cart);
    }

    // Orders
    public async Task<List<Order>> GetAllOrdersAsync() => 
        await _context.Orders.Find(_ => true).ToListAsync();

    public async Task<List<Order>> GetCustomerOrdersAsync(int customerId) => 
        await _context.Orders.Find(o => o.CustomerId == customerId).ToListAsync();

    public async Task<Order?> GetOrderAsync(int orderId) => 
        await _context.Orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

    public async Task<Order> CreateOrderAsync(int customerId)
    {
        var customer = await GetCustomerAsync(customerId);
        var cart = await GetCartAsync(customerId);
        
        if (customer == null || !cart.Items.Any())
            throw new InvalidOperationException("Invalid customer or empty cart");

        var order = new Order
        {
            OrderId = _nextOrderId++,
            CustomerId = customerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = cart.Items.Select(ci => new OrderItem
            {
                OrderItemId = _nextOrderItemId++,
                ProductId = ci.ProductId,
                Product = ci.Product,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice
            }).ToList()
        };

        // Apply customer discount
        order.DiscountAmount = order.SubTotal * (customer.DiscountPercentage / 100);

        // Update stock quantities
        foreach (var item in order.OrderItems)
        {
            var product = await GetProductAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
                await UpdateProductAsync(product);
            }
        }

        await _context.Orders.InsertOneAsync(order);
        
        // Clear the cart
        await ClearCartAsync(customerId);
        
        return order;
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
        var update = Builders<Order>.Update.Set(o => o.Status, status);
        await _context.Orders.UpdateOneAsync(filter, update);
    }

    // Seeding data
    public async Task SeedDataAsync()
    {
        // Check if data already exists
        var productsCount = await _context.Products.CountDocumentsAsync(_ => true);
        if (productsCount > 0) return; // Data already seeded

        // Seed Products
        var products = new[]
        {
            // Cookies
            new Product { ProductId = _nextProductId++, Name = "Chocolate Chip Cookies", Description = "Classic chocolate chip cookies", Price = 8.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/chocolate-chip-cookies.jpg", StockQuantity = 50 },
            new Product { ProductId = _nextProductId++, Name = "Oatmeal Raisin Cookies", Description = "Chewy oatmeal cookies with raisins", Price = 7.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/oatmeal-raisin-cookies.jpg", StockQuantity = 40 },
            new Product { ProductId = _nextProductId++, Name = "Sugar Cookies", Description = "Sweet and simple sugar cookies", Price = 6.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/sugar-cookies.jpg", StockQuantity = 60 },
            new Product { ProductId = _nextProductId++, Name = "Double Chocolate Cookies", Description = "Rich double chocolate fudge cookies", Price = 9.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/double-chocolate-cookies.jpg", StockQuantity = 35 },
            
            // Ice Cream
            new Product { ProductId = _nextProductId++, Name = "Vanilla Ice Cream", Description = "Classic vanilla ice cream", Price = 12.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/vanilla-ice-cream.jpg", StockQuantity = 25 },
            new Product { ProductId = _nextProductId++, Name = "Chocolate Ice Cream", Description = "Rich chocolate ice cream", Price = 12.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/chocolate-ice-cream.jpg", StockQuantity = 30 },
            new Product { ProductId = _nextProductId++, Name = "Strawberry Ice Cream", Description = "Fresh strawberry ice cream", Price = 13.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/strawberry-ice-cream.jpg", StockQuantity = 20 },
            new Product { ProductId = _nextProductId++, Name = "Mint Chocolate Chip Ice Cream", Description = "Refreshing mint with chocolate chips", Price = 14.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/mint-chocolate-chip.jpg", StockQuantity = 22 },
            
            // Candy
            new Product { ProductId = _nextProductId++, Name = "Gummy Bears", Description = "Colorful fruit-flavored gummy bears", Price = 4.99m, Category = ProductCategory.Candy, ImageUrl = "/images/gummy-bears.jpg", StockQuantity = 100 },
            new Product { ProductId = _nextProductId++, Name = "Chocolate Truffles", Description = "Premium dark chocolate truffles", Price = 19.99m, Category = ProductCategory.Candy, ImageUrl = "/images/chocolate-truffles.jpg", StockQuantity = 15 },
            new Product { ProductId = _nextProductId++, Name = "Lollipops", Description = "Assorted fruit flavor lollipops", Price = 3.99m, Category = ProductCategory.Candy, ImageUrl = "/images/lollipops.jpg", StockQuantity = 80 },
            new Product { ProductId = _nextProductId++, Name = "Chocolate Bar", Description = "Swiss milk chocolate bar", Price = 5.99m, Category = ProductCategory.Candy, ImageUrl = "/images/chocolate-bar.jpg", StockQuantity = 75 }
        };

        await _context.Products.InsertManyAsync(products);

        // Seed Customers
        var customers = new[]
        {
            new Customer { CustomerId = _nextCustomerId++, Name = "John Doe", Email = "john@example.com", Phone = "555-0101", Type = CustomerType.Regular, DiscountPercentage = 0 },
            new Customer { CustomerId = _nextCustomerId++, Name = "Jane Smith", Email = "jane@example.com", Phone = "555-0102", Type = CustomerType.Premium, DiscountPercentage = 10 },
            new Customer { CustomerId = _nextCustomerId++, Name = "Bob Johnson", Email = "bob@example.com", Phone = "555-0103", Type = CustomerType.VIP, DiscountPercentage = 20 }
        };

        await _context.Customers.InsertManyAsync(customers);
    }
}
