# Quick Setup Guide

This guide will help you get the SweetStoreAPI running with MongoDB quickly.

## 1. Start MongoDB

### Option A: Using Docker (Recommended)
```bash
# Start MongoDB and Mongo Express
docker-compose up -d

# Verify containers are running
docker-compose ps
```

### Option B: Local MongoDB
Ensure MongoDB is running on port 27017.

## 2. Run the Application

```bash
# Restore packages and run
dotnet run
```

The application will:
- Automatically connect to MongoDB
- Create the database and collections if they don't exist
- Seed initial data (products and customers)
- Start the API server

## 3. Access the Application

- **API**: https://localhost:5001 or http://localhost:5000
- **Swagger UI**: https://localhost:5001/swagger
- **Mongo Express** (if using Docker): http://localhost:8081
  - Username: admin
  - Password: password

## 4. Test the API

### Get all products
```bash
curl https://localhost:5001/api/products
```

### Get a specific customer
```bash
curl https://localhost:5001/api/customers/1
```

### Get customer's shopping cart
```bash
curl https://localhost:5001/api/cart/1
```

## 5. Troubleshooting

### MongoDB Connection Issues
- Verify MongoDB is running: `docker-compose ps`
- Check logs: `docker-compose logs mongodb`
- Restart services: `docker-compose restart`

### Application Issues
- Check if MongoDB is accessible on port 27017
- Verify connection string in appsettings.json
- Check application logs for detailed error messages

## Database Collections

After startup, your MongoDB will contain:
- **Products**: 12 sample products (cookies, ice cream, candy)
- **Customers**: 3 sample customers with different discount tiers
- **Orders**: Empty (created through API usage)
- **ShoppingCarts**: Empty (created when customers add items)

## API Features

- **Products**: Browse by category, get details
- **Customers**: Customer management with discount tiers
- **Shopping Cart**: Add/remove items, update quantities
- **Orders**: Create orders from cart, track order history
