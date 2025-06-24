using SweetStoreAPI.Models;
using SweetStoreAPI.DTOs;

namespace SweetStoreAPI.Services;

public static class MappingService
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Category = product.Category.ToString(),
            ImageUrl = product.ImageUrl,
            IsAvailable = product.IsAvailable,
            StockQuantity = product.StockQuantity
        };
    }

    public static CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.CustomerId,
            Name = customer.Name,
            Email = customer.Email,
            Phone = customer.Phone,
            Type = customer.Type.ToString(),
            DiscountPercentage = customer.DiscountPercentage
        };
    }

    public static Customer FromDto(CreateCustomerDto dto)
    {
        return new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            Type = Enum.Parse<CustomerType>(dto.Type, true)
        };
    }

    public static CartDto ToDto(ShoppingCart cart, Customer customer)
    {
        var cartDto = new CartDto
        {
            CustomerId = cart.CustomerId,
            SubTotal = cart.SubTotal,
            Items = cart.Items.Select(ToDto).ToList()
        };

        cartDto.DiscountAmount = cartDto.SubTotal * (customer.DiscountPercentage / 100);
        cartDto.TotalAmount = cartDto.SubTotal - cartDto.DiscountAmount;

        return cartDto;
    }

    public static CartItemDto ToDto(CartItem item)
    {
        return new CartItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice,
            ImageUrl = item.Product.ImageUrl
        };
    }

    public static OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.OrderId,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.Name,
            OrderDate = order.OrderDate,
            SubTotal = order.SubTotal,
            DiscountAmount = order.DiscountAmount,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            OrderItems = order.OrderItems.Select(ToDto).ToList()
        };
    }

    public static OrderItemDto ToDto(OrderItem item)
    {
        return new OrderItemDto
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TotalPrice = item.TotalPrice
        };
    }
}
