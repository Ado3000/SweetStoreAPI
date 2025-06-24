#!/bin/bash
# Setup script for SweetStore API with MongoDB

echo "🚀 Setting up SweetStore API with MongoDB..."

# Stop any existing containers
echo "📦 Stopping existing containers..."
docker-compose down

# Remove old volumes to ensure clean setup
echo "🧹 Cleaning up old data..."
docker volume rm sweetstoreapi_mongodb_data 2>/dev/null || true

# Start MongoDB and Mongo Express
echo "🐳 Starting MongoDB and Mongo Express..."
docker-compose up -d

# Wait for MongoDB to be ready
echo "⏳ Waiting for MongoDB to be ready..."
sleep 10

# Check if MongoDB is running
if docker ps | grep -q sweetstore-mongodb; then
    echo "✅ MongoDB is running!"
    echo "✅ Mongo Express is available at: http://localhost:8081"
    echo "   Username: admin"
    echo "   Password: password"
else
    echo "❌ Failed to start MongoDB"
    exit 1
fi

# Build and run the application
echo "🔨 Building the application..."
dotnet build

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "🎯 You can now run the application with: dotnet run"
    echo "📖 API will be available at: https://localhost:5001"
    echo "📚 Swagger documentation: https://localhost:5001/swagger"
else
    echo "❌ Build failed"
    exit 1
fi

echo ""
echo "🎉 Setup complete!"
echo ""
echo "To start the application:"
echo "  dotnet run"
echo ""
echo "To stop MongoDB:"
echo "  docker-compose down"
