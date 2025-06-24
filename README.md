# SweetStoreAPI

A .NET 9 Web API for managing a sweet store, including products, customers, orders, and shopping cart functionality.

## Features

- Product management
- Customer management
- Order processing
- Shopping cart functionality
- Data Transfer Objects (DTOs) for API responses
- Service layer architecture

## Project Structure

```
SweetStoreAPI/
├── DTOs/              # Data Transfer Objects
├── Models/            # Domain models
├── Services/          # Business logic services
├── Properties/        # Launch settings
├── Program.cs         # Application entry point
└── SweetStoreAPI.http # HTTP request examples
```

## Getting Started

### Prerequisites

- .NET 9 SDK

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

## API Documentation

The API includes Swagger/OpenAPI documentation available at `/swagger` when running in development mode.

## Models

- **Product**: Sweet store products
- **Customer**: Customer information
- **Order**: Order details and items
- **ShoppingCart**: Shopping cart functionality

## Services

- **DataService**: Data access and management
- **MappingService**: Object mapping between models and DTOs

## Configuration

Configuration settings are managed through:
- `appsettings.json`: Production settings
- `appsettings.Development.json`: Development-specific settings
