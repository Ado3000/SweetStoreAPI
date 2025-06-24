using SweetStoreAPI.Models;
using SweetStoreAPI.DTOs;
using SweetStoreAPI.Services;
using SweetStoreAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<MongoDataService>();

// Add CORS policy for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReact");
app.UseHttpsRedirection();

// Seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dataService = scope.ServiceProvider.GetRequiredService<MongoDataService>();
    await dataService.SeedDataAsync();
}

// Products API
app.MapGet("/api/products", async (MongoDataService dataService) =>
{
    var products = await dataService.GetAllProductsAsync();
    return Results.Ok(products.Select(MappingService.ToDto));
})
.WithName("GetAllProducts")
.WithTags("Products");

app.MapGet("/api/products/category/{category}", async (string category, MongoDataService dataService) =>
{
    if (!Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
    {
        return Results.BadRequest("Invalid category");
    }
    
    var products = await dataService.GetProductsByCategoryAsync(categoryEnum);
    return Results.Ok(products.Select(MappingService.ToDto));
})
.WithName("GetProductsByCategory")
.WithTags("Products");

app.MapGet("/api/products/{id}", async (int id, MongoDataService dataService) =>
{
    var product = await dataService.GetProductAsync(id);
    if (product == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(product));
})
.WithName("GetProduct")
.WithTags("Products");

// Customers API
app.MapGet("/api/customers", async (MongoDataService dataService) =>
{
    var customers = await dataService.GetAllCustomersAsync();
    return Results.Ok(customers.Select(MappingService.ToDto));
})
.WithName("GetAllCustomers")
.WithTags("Customers");

app.MapGet("/api/customers/{id}", async (int id, MongoDataService dataService) =>
{
    var customer = await dataService.GetCustomerAsync(id);
    if (customer == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(customer));
})
.WithName("GetCustomer")
.WithTags("Customers");

app.MapPost("/api/customers", async (CreateCustomerDto customerDto, MongoDataService dataService) =>
{
    try
    {
        var customer = MappingService.FromDto(customerDto);
        var createdCustomer = await dataService.CreateCustomerAsync(customer);
        return Results.Created($"/api/customers/{createdCustomer.CustomerId}", MappingService.ToDto(createdCustomer));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateCustomer")
.WithTags("Customers");

// Shopping Cart API
app.MapGet("/api/cart/{customerId}", async (int customerId, MongoDataService dataService) =>
{
    var customer = await dataService.GetCustomerAsync(customerId);
    if (customer == null)
        return Results.NotFound("Customer not found");
    
    var cart = await dataService.GetCartAsync(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer));
})
.WithName("GetCart")
.WithTags("Cart");

app.MapPost("/api/cart/add", async (AddToCartDto addToCartDto, MongoDataService dataService) =>
{
    try
    {
        await dataService.AddToCartAsync(addToCartDto.CustomerId, addToCartDto.ProductId, addToCartDto.Quantity);
        
        var customer = await dataService.GetCustomerAsync(addToCartDto.CustomerId);
        var cart = await dataService.GetCartAsync(addToCartDto.CustomerId);
        return Results.Ok(MappingService.ToDto(cart, customer!));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("AddToCart")
.WithTags("Cart");

app.MapDelete("/api/cart/{customerId}/items/{productId}", async (int customerId, int productId, MongoDataService dataService) =>
{
    await dataService.RemoveFromCartAsync(customerId, productId);
    
    var customer = await dataService.GetCustomerAsync(customerId);
    var cart = await dataService.GetCartAsync(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer!));
})
.WithName("RemoveFromCart")
.WithTags("Cart");

app.MapPut("/api/cart/{customerId}/items/{productId}/quantity/{quantity}", async (int customerId, int productId, int quantity, MongoDataService dataService) =>
{
    await dataService.UpdateCartItemQuantityAsync(customerId, productId, quantity);
    
    var customer = await dataService.GetCustomerAsync(customerId);
    var cart = await dataService.GetCartAsync(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer!));
})
.WithName("UpdateCartItemQuantity")
.WithTags("Cart");

// Orders API
app.MapGet("/api/orders", async (MongoDataService dataService) =>
{
    var orders = await dataService.GetAllOrdersAsync();
    return Results.Ok(orders.Select(MappingService.ToDto));
})
.WithName("GetAllOrders")
.WithTags("Orders");

app.MapGet("/api/orders/customer/{customerId}", async (int customerId, MongoDataService dataService) =>
{
    var orders = await dataService.GetCustomerOrdersAsync(customerId);
    return Results.Ok(orders.Select(MappingService.ToDto));
})
.WithName("GetCustomerOrders")
.WithTags("Orders");

app.MapGet("/api/orders/{id}", async (int id, MongoDataService dataService) =>
{
    var order = await dataService.GetOrderAsync(id);
    if (order == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(order));
})
.WithName("GetOrder")
.WithTags("Orders");

app.MapPost("/api/orders", async (CreateOrderDto orderDto, MongoDataService dataService) =>
{
    try
    {
        var order = await dataService.CreateOrderAsync(orderDto.CustomerId);
        return Results.Created($"/api/orders/{order.OrderId}", MappingService.ToDto(order));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateOrder")
.WithTags("Orders");

app.Run();
