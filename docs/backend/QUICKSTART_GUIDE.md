# 🚀 Backend Quick Start Guide

Get up and running with the .NET 8 Web API backend in under 5 minutes.

## 📋 Prerequisites

- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 14+** - [Download here](https://www.postgresql.org/download/)
- **Git** - For repository management
- **Visual Studio Code** - Recommended IDE with C# extension

## ⚡ 1-Minute Setup

### 1. Clone and Navigate
```bash
git clone https://github.com/kalviumcommunity/Container-Tracking-and-Port-Operations-Maersk-Hackathon.git
cd Container-Tracking-and-Port-Operations-Maersk-Hackathon/backend
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Configure Database
```bash
# Create PostgreSQL database
createdb container_tracking_dev

# Update connection string in appsettings.Development.json
# "DefaultConnection": "Host=localhost;Database=container_tracking_dev;Username=postgres;Password=your_password"
```

### 4. Run Migrations
```bash
dotnet ef database update
```

### 5. Start the Server
```bash
dotnet run
```

**🎉 Backend ready at:** http://localhost:5221  
**📊 API Documentation:** http://localhost:5221/swagger

## 🔐 Test Authentication

### Default Credentials
- **Username:** `admin`
- **Password:** `Admin123!`

### Quick API Test
```bash
# Get JWT token
curl -X POST "http://localhost:5221/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'

# Test API endpoint
curl -X GET "http://localhost:5221/api/containers" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **.NET** | 8.0 | Web API framework |
| **Entity Framework Core** | 8.0 | ORM and database access |
| **PostgreSQL** | 14+ | Primary database |
| **JWT** | Latest | Authentication tokens |
| **Swagger/OpenAPI** | 3.0 | API documentation |
| **Serilog** | Latest | Structured logging |

## 📊 Seeded Data Overview

The system comes pre-loaded with realistic test data:

### 🌍 Global Operations
- **25 Major Ports** - Copenhagen, Shanghai, Los Angeles, Dubai, Rotterdam, Singapore, Hamburg, Antwerp, Rotterdam, etc.
- **60+ Ships** - Real vessels from Maersk, MSC, COSCO, Evergreen, HMM, OOCL, CMA CGM
- **300+ Containers** - Various types: DRY, REEFER, TANK, OPEN_TOP with realistic cargo

### 🏗️ Operational Data
- **120+ Berth Assignments** - Realistic scheduling across all ports
- **Container Movements** - Complete tracking history
- **User Roles** - Admin, Port Manager, Operator, Viewer with proper permissions

## 🧪 Development Commands

### Database Operations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Drop database (development only)
dotnet ef database drop

# Reset database with fresh data
dotnet ef database drop
dotnet ef database update
```

### Development Server
```bash
# Run with file watching (auto-restart on changes)
dotnet watch run

# Run with specific environment
dotnet run --environment Development

# Build for production
dotnet build --configuration Release
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Tests/Unit/UnitTests.csproj
```

## 🔍 Troubleshooting

### Common Issues

#### Database Connection Failed
```bash
# Check PostgreSQL is running
pg_isready -h localhost -p 5432

# Test direct connection
psql -h localhost -U postgres -d container_tracking_dev

# Update connection string in appsettings.Development.json
```

#### Port Already in Use
```bash
# Check what's using port 5221
netstat -ano | findstr :5221

# Kill process (Windows)
taskkill /PID <process_id> /F

# Or change port in launchSettings.json
```

#### Migration Errors
```bash
# Remove last migration
dotnet ef migrations remove

# Clean migrations and start fresh
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 📚 Next Steps

### For Development
1. **Review API Endpoints** - Check Swagger UI at http://localhost:5221/swagger
2. **Explore Database** - Connect with your favorite PostgreSQL client
3. **Test Authentication** - Use Postman collections in `docs/backend/`
4. **Read Architecture** - [Backend Architecture Guide](./BACKEND_ARCHITECTURE.md)

### For Production
1. **Environment Configuration** - [Deployment Guide](./DEPLOYMENT_GUIDE.md)
2. **Security Setup** - Review JWT configuration and HTTPS
3. **Database Migration** - Production database setup procedures
4. **Monitoring Setup** - Application Insights or equivalent

### For Testing
1. **Unit Tests** - [Testing Guide](./TESTING_GUIDE.md)
2. **Integration Tests** - API endpoint testing strategies  
3. **Postman Collections** - Import and run test suites
4. **Load Testing** - Performance testing guidelines

## 🎯 Quick Verification Checklist

- [ ] Backend server starts without errors
- [ ] Swagger UI loads at http://localhost:5221/swagger
- [ ] Database connection successful
- [ ] Login with admin credentials works
- [ ] API endpoints return data
- [ ] Health check endpoint responds

**Ready to build amazing maritime solutions!** 🚢

For detailed development information, see the [Development Setup Guide](./DEVELOPMENT_SETUP.md).