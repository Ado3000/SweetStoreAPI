using SweetStoreAPI.Models;

namespace SweetStoreAPI.Services;

public class DataService
{
    private static List<Product> _products = new();
    private static List<Customer> _customers = new();
    private static List<Order> _orders = new();
    private static Dictionary<int, ShoppingCart> _carts = new();
    private static int _nextProductId = 1;
    private static int _nextCustomerId = 1;
    private static int _nextOrderId = 1;

    static DataService()
    {
        SeedData();
    }

    // Products
    public List<Product> GetAllProducts() => _products;
    
    public List<Product> GetProductsByCategory(ProductCategory category) => 
        _products.Where(p => p.Category == category).ToList();
    
    public Product? GetProduct(int id) => _products.FirstOrDefault(p => p.Id == id);

    // Customers
    public List<Customer> GetAllCustomers() => _customers;
    
    public Customer? GetCustomer(int id) => _customers.FirstOrDefault(c => c.Id == id);
    
    public Customer CreateCustomer(Customer customer)
    {
        customer.Id = _nextCustomerId++;
        customer.DiscountPercentage = customer.Type switch
        {
            CustomerType.Regular => 0,
            CustomerType.Premium => 10,
            CustomerType.VIP => 20,
            _ => 0
        };
        _customers.Add(customer);
        return customer;
    }

    // Shopping Cart
    public ShoppingCart GetCart(int customerId)
    {
        if (!_carts.ContainsKey(customerId))
        {
            _carts[customerId] = new ShoppingCart { CustomerId = customerId };
        }
        return _carts[customerId];
    }

    public void AddToCart(int customerId, int productId, int quantity)
    {
        var cart = GetCart(customerId);
        var product = GetProduct(productId);
        
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
    }

    public void RemoveFromCart(int customerId, int productId)
    {
        var cart = GetCart(customerId);
        cart.Items.RemoveAll(i => i.ProductId == productId);
    }

    public void UpdateCartItemQuantity(int customerId, int productId, int quantity)
    {
        var cart = GetCart(customerId);
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
        }
    }

    // Orders
    public List<Order> GetAllOrders() => _orders;
    
    public List<Order> GetCustomerOrders(int customerId) => 
        _orders.Where(o => o.CustomerId == customerId).ToList();
    
    public Order? GetOrder(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public Order CreateOrder(int customerId)
    {
        var customer = GetCustomer(customerId);
        var cart = GetCart(customerId);
        
        if (customer == null || !cart.Items.Any())
            throw new InvalidOperationException("Invalid customer or empty cart");

        var order = new Order
        {
            Id = _nextOrderId++,
            CustomerId = customerId,
            Customer = customer,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            OrderItems = cart.Items.Select(ci => new OrderItem
            {
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
            var product = GetProduct(item.ProductId);
            if (product != null)
            {
                product.StockQuantity -= item.Quantity;
            }
        }

        _orders.Add(order);
        
        // Clear the cart
        cart.Items.Clear();
        
        return order;
    }

    private static void SeedData()
    {
        // Seed Products
        _products.AddRange(new[]
        {
            // Cookies
            new Product { Id = _nextProductId++, Name = "Chocolate Chip Cookies", Description = "Classic chocolate chip cookies", Price = 8.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/chocolate-chip-cookies.jpg", StockQuantity = 50 },
            new Product { Id = _nextProductId++, Name = "Oatmeal Raisin Cookies", Description = "Chewy oatmeal cookies with raisins", Price = 7.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/oatmeal-raisin-cookies.jpg", StockQuantity = 40 },
            new Product { Id = _nextProductId++, Name = "Sugar Cookies", Description = "Sweet and simple sugar cookies", Price = 6.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/sugar-cookies.jpg", StockQuantity = 60 },
            new Product { Id = _nextProductId++, Name = "Double Chocolate Cookies", Description = "Rich double chocolate fudge cookies", Price = 9.99m, Category = ProductCategory.Cookies, ImageUrl = "/images/double-chocolate-cookies.jpg", StockQuantity = 35 },
            
            // Ice Cream
            new Product { Id = _nextProductId++, Name = "Vanilla Ice Cream", Description = "Classic vanilla ice cream", Price = 12.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/vanilla-ice-cream.jpg", StockQuantity = 25 },
            new Product { Id = _nextProductId++, Name = "Chocolate Ice Cream", Description = "Rich chocolate ice cream", Price = 12.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/chocolate-ice-cream.jpg", StockQuantity = 30 },
            new Product { Id = _nextProductId++, Name = "Strawberry Ice Cream", Description = "Fresh strawberry ice cream", Price = 13.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/strawberry-ice-cream.jpg", StockQuantity = 20 },
            new Product { Id = _nextProductId++, Name = "Mint Chocolate Chip Ice Cream", Description = "Refreshing mint with chocolate chips", Price = 14.99m, Category = ProductCategory.IceCream, ImageUrl = "/images/mint-chocolate-chip.jpg", StockQuantity = 22 },
            
            // Candy
            new Product { Id = _nextProductId++, Name = "Gummy Bears", Description = "Colorful fruit-flavored gummy bears", Price = 4.99m, Category = ProductCategory.Candy, ImageUrl = "/images/gummy-bears.jpg", StockQuantity = 100 },
            new Product { Id = _nextProductId++, Name = "Chocolate Truffles", Description = "Premium dark chocolate truffles", Price = 19.99m, Category = ProductCategory.Candy, ImageUrl = "/images/chocolate-truffles.jpg", StockQuantity = 15 },
            new Product { Id = _nextProductId++, Name = "Lollipops", Description = "Assorted fruit flavor lollipops", Price = 3.99m, Category = ProductCategory.Candy, ImageUrl = "/images/lollipops.jpg", StockQuantity = 80 },
            new Product { Id = _nextProductId++, Name = "Chocolate Bar", Description = "Swiss milk chocolate bar", Price = 5.99m, Category = ProductCategory.Candy, ImageUrl = "/images/chocolate-bar.jpg", StockQuantity = 75 }
        });

        // Seed Customers
        _customers.AddRange(new[]
        {
            new Customer { Id = _nextCustomerId++, Name = "John Doe", Email = "john@example.com", Phone = "555-0101", Type = CustomerType.Regular, DiscountPercentage = 0 },
            new Customer { Id = _nextCustomerId++, Name = "Jane Smith", Email = "jane@example.com", Phone = "555-0102", Type = CustomerType.Premium, DiscountPercentage = 10 },
            new Customer { Id = _nextCustomerId++, Name = "Bob Johnson", Email = "bob@example.com", Phone = "555-0103", Type = CustomerType.VIP, DiscountPercentage = 20 }
        });
    }
}
