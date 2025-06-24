# SweetStoreAPI

A .NET 9 Web API for managing a sweet store, including products, customers, orders, and shopping cart functionality. The application uses MongoDB as the database with MongoDB Driver for data persistence.

## Features

- Product management with MongoDB persistence
- Customer management
- Order processing
- Shopping cart functionality
- Data Transfer Objects (DTOs) for API responses
- Service layer architecture with async/await pattern
- MongoDB integration with proper indexing

## Project Structure

```
SweetStoreAPI/
├── Data/              # MongoDB context and settings
├── DTOs/              # Data Transfer Objects
├── Models/            # Domain models with MongoDB attributes
├── Services/          # Business logic services
├── Properties/        # Launch settings
├── Program.cs         # Application entry point
├── docker-compose.yml # MongoDB setup with Docker
└── SweetStoreAPI.http # HTTP request examples
```

## Technology Stack

- **.NET 9**: Latest .NET framework
- **MongoDB**: Document database
- **MongoDB.Driver**: Official MongoDB .NET driver
- **ASP.NET Core**: Web API framework
- **Docker**: For MongoDB containerization

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker and Docker Compose (for MongoDB)
- Or MongoDB installed locally

### Setting up MongoDB

#### Option 1: Using Docker (Recommended)
1. Start MongoDB using Docker Compose:
   ```bash
   docker-compose up -d
   ```
   This will start:
   - MongoDB on port 27017
   - Mongo Express (web UI) on port 8081

2. Access Mongo Express at http://localhost:8081
   - Username: admin
   - Password: password

#### Option 2: Local MongoDB Installation
1. Install MongoDB locally
2. Ensure MongoDB is running on port 27017
3. The application will automatically create the database and collections

### Running the Application

1. Clone the repository
2. Navigate to the project directory
3. Start MongoDB (using Docker Compose or local installation)
4. Run the application:
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger` (in development mode)

## Database Configuration

### Connection Strings
- **Development**: `mongodb://localhost:27017` (SweetStoreDB_Dev)
- **Production**: `mongodb://localhost:27017` (SweetStoreDB)

### Collections
- **Products**: Store product information
- **Customers**: Store customer data
- **Orders**: Store order details and items
- **ShoppingCarts**: Store shopping cart data

### Indexes
The application automatically creates indexes for:
- Product ID, category, and name
- Customer ID and email
- Order ID, customer ID, and order date
- Shopping cart customer ID

## API Documentation

The API includes Swagger/OpenAPI documentation available at `/swagger` when running in development mode.

### Main Endpoints

#### Products
- `GET /api/products` - Get all products
- `GET /api/products/category/{category}` - Get products by category
- `GET /api/products/{id}` - Get product by ID

#### Customers
- `GET /api/customers` - Get all customers
- `GET /api/customers/{id}` - Get customer by ID
- `POST /api/customers` - Create new customer

#### Shopping Cart
- `GET /api/cart/{customerId}` - Get customer's cart
- `POST /api/cart/add` - Add item to cart
- `DELETE /api/cart/{customerId}/items/{productId}` - Remove item from cart
- `PUT /api/cart/{customerId}/items/{productId}/quantity/{quantity}` - Update item quantity

#### Orders
- `GET /api/orders` - Get all orders
- `GET /api/orders/customer/{customerId}` - Get customer's orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create new order

## Models

- **Product**: Sweet store products with MongoDB ObjectId
- **Customer**: Customer information with discount tiers
- **Order**: Order details with items and customer information
- **ShoppingCart**: Shopping cart with items and totals

## Services

- **MongoDataService**: Async data access layer using MongoDB Driver
- **MappingService**: Object mapping between models and DTOs
- **MongoDbContext**: MongoDB database context and collections

## Configuration

### appsettings.json
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "SweetStoreDB",
    "ProductsCollectionName": "Products",
    "CustomersCollectionName": "Customers",
    "OrdersCollectionName": "Orders",
    "ShoppingCartsCollectionName": "ShoppingCarts"
  }
}
```

## Development

### Data Seeding
The application automatically seeds initial data on startup:
- 12 sample products across 3 categories (Cookies, Ice Cream, Candy)
- 3 sample customers with different discount tiers

### MongoDB Management
- Use Mongo Express web interface at http://localhost:8081
- Or connect directly using MongoDB Compass or any MongoDB client
- Connection string: `mongodb://localhost:27017`

## Docker Support

The project includes a `docker-compose.yml` file for easy MongoDB setup:

```bash
# Start MongoDB and Mongo Express
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs mongodb
```
