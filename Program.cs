using SweetStoreAPI.Models;
using SweetStoreAPI.DTOs;
using SweetStoreAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSingleton<DataService>();

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

var dataService = app.Services.GetRequiredService<DataService>();

// Products API
app.MapGet("/api/products", () =>
{
    var products = dataService.GetAllProducts();
    return Results.Ok(products.Select(MappingService.ToDto));
})
.WithName("GetAllProducts")
.WithTags("Products");

app.MapGet("/api/products/category/{category}", (string category) =>
{
    if (!Enum.TryParse<ProductCategory>(category, true, out var categoryEnum))
    {
        return Results.BadRequest("Invalid category");
    }
    
    var products = dataService.GetProductsByCategory(categoryEnum);
    return Results.Ok(products.Select(MappingService.ToDto));
})
.WithName("GetProductsByCategory")
.WithTags("Products");

app.MapGet("/api/products/{id}", (int id) =>
{
    var product = dataService.GetProduct(id);
    if (product == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(product));
})
.WithName("GetProduct")
.WithTags("Products");

// Customers API
app.MapGet("/api/customers", () =>
{
    var customers = dataService.GetAllCustomers();
    return Results.Ok(customers.Select(MappingService.ToDto));
})
.WithName("GetAllCustomers")
.WithTags("Customers");

app.MapGet("/api/customers/{id}", (int id) =>
{
    var customer = dataService.GetCustomer(id);
    if (customer == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(customer));
})
.WithName("GetCustomer")
.WithTags("Customers");

app.MapPost("/api/customers", (CreateCustomerDto customerDto) =>
{
    try
    {
        var customer = MappingService.FromDto(customerDto);
        var createdCustomer = dataService.CreateCustomer(customer);
        return Results.Created($"/api/customers/{createdCustomer.Id}", MappingService.ToDto(createdCustomer));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateCustomer")
.WithTags("Customers");

// Shopping Cart API
app.MapGet("/api/cart/{customerId}", (int customerId) =>
{
    var customer = dataService.GetCustomer(customerId);
    if (customer == null)
        return Results.NotFound("Customer not found");
    
    var cart = dataService.GetCart(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer));
})
.WithName("GetCart")
.WithTags("Cart");

app.MapPost("/api/cart/add", (AddToCartDto addToCartDto) =>
{
    try
    {
        dataService.AddToCart(addToCartDto.CustomerId, addToCartDto.ProductId, addToCartDto.Quantity);
        
        var customer = dataService.GetCustomer(addToCartDto.CustomerId);
        var cart = dataService.GetCart(addToCartDto.CustomerId);
        return Results.Ok(MappingService.ToDto(cart, customer!));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("AddToCart")
.WithTags("Cart");

app.MapDelete("/api/cart/{customerId}/items/{productId}", (int customerId, int productId) =>
{
    dataService.RemoveFromCart(customerId, productId);
    
    var customer = dataService.GetCustomer(customerId);
    var cart = dataService.GetCart(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer!));
})
.WithName("RemoveFromCart")
.WithTags("Cart");

app.MapPut("/api/cart/{customerId}/items/{productId}/quantity/{quantity}", (int customerId, int productId, int quantity) =>
{
    dataService.UpdateCartItemQuantity(customerId, productId, quantity);
    
    var customer = dataService.GetCustomer(customerId);
    var cart = dataService.GetCart(customerId);
    return Results.Ok(MappingService.ToDto(cart, customer!));
})
.WithName("UpdateCartItemQuantity")
.WithTags("Cart");

// Orders API
app.MapGet("/api/orders", () =>
{
    var orders = dataService.GetAllOrders();
    return Results.Ok(orders.Select(MappingService.ToDto));
})
.WithName("GetAllOrders")
.WithTags("Orders");

app.MapGet("/api/orders/customer/{customerId}", (int customerId) =>
{
    var orders = dataService.GetCustomerOrders(customerId);
    return Results.Ok(orders.Select(MappingService.ToDto));
})
.WithName("GetCustomerOrders")
.WithTags("Orders");

app.MapGet("/api/orders/{id}", (int id) =>
{
    var order = dataService.GetOrder(id);
    if (order == null)
        return Results.NotFound();
    
    return Results.Ok(MappingService.ToDto(order));
})
.WithName("GetOrder")
.WithTags("Orders");

app.MapPost("/api/orders", (CreateOrderDto orderDto) =>
{
    try
    {
        var order = dataService.CreateOrder(orderDto.CustomerId);
        return Results.Created($"/api/orders/{order.Id}", MappingService.ToDto(order));
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateOrder")
.WithTags("Orders");

app.Run();
