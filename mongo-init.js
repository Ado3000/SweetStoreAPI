// MongoDB initialization script
db = db.getSiblingDB('SweetStoreDB');

// Create collections with indexes
db.createCollection('Products');
db.createCollection('Customers'); 
db.createCollection('Orders');
db.createCollection('ShoppingCarts');

// Create indexes for better performance
db.Products.createIndex({ "productId": 1 }, { unique: true });
db.Products.createIndex({ "category": 1 });
db.Products.createIndex({ "name": 1 });

db.Customers.createIndex({ "customerId": 1 }, { unique: true });
db.Customers.createIndex({ "email": 1 }, { unique: true });

db.Orders.createIndex({ "orderId": 1 }, { unique: true });
db.Orders.createIndex({ "customerId": 1 });
db.Orders.createIndex({ "orderDate": 1 });

db.ShoppingCarts.createIndex({ "customerId": 1 }, { unique: true });

print('MongoDB initialized successfully for SweetStoreDB');
