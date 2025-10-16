# ⚙️ Backend Development Setup Guide

## Prerequisites

Before setting up the backend development environment, ensure you have the following installed:

### Required Software
- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 14+** - [Download](https://www.postgresql.org/download/)
- **Git** - [Download](https://git-scm.com/downloads)
- **Visual Studio Code** or **Visual Studio 2022**

### Recommended Tools
- **Docker Desktop** - For containerized PostgreSQL
- **Postman** - For API testing
- **pgAdmin** - PostgreSQL administration
- **Azure Data Studio** - Database management

## 🚀 Quick Start Setup

### 1. Clone Repository
```bash
git clone https://github.com/kalviumcommunity/Container-Tracking-and-Port-Operations-Maersk-Hackathon.git
cd Container-Tracking-and-Port-Operations-Maersk-Hackathon/backend
```

### 2. Install .NET Dependencies
```bash
# Restore NuGet packages
dotnet restore

# Install Entity Framework CLI tools (global)
dotnet tool install --global dotnet-ef

# Verify installation
dotnet --version
dotnet ef --version
```

### 3. Database Setup

#### Option A: Local PostgreSQL Installation
```bash
# Install PostgreSQL (Windows - using Chocolatey)
choco install postgresql

# Install PostgreSQL (macOS - using Homebrew)
brew install postgresql

# Install PostgreSQL (Ubuntu/Debian)
sudo apt-get install postgresql postgresql-contrib
```

#### Option B: Docker PostgreSQL (Recommended)
```bash
# Pull and run PostgreSQL container
docker run --name postgres-container \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=ContainerTracking \
  -p 5433:5432 \
  -d postgres:14

# Verify container is running
docker ps
```

### 4. Environment Configuration

#### Create Environment Files
```bash
# Copy example environment file
cp .env.example .env
```

#### Update `.env` file:
```env
# Database Configuration
ConnectionStrings__DefaultConnection=Host=localhost;Port=5433;Database=ContainerTracking;Username=postgres;Password=postgres

# JWT Configuration
JWT__SecretKey=your-super-secret-jwt-key-min-32-characters-long
JWT__Issuer=MaerskContainerTracking
JWT__Audience=MaerskContainerTracking
JWT__ExpiryMinutes=1440

# Logging Configuration
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning

# CORS Configuration
CORS__AllowedOrigins=http://localhost:5173,http://localhost:3000

# Environment
ASPNETCORE_ENVIRONMENT=Development
```

#### Update `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=ContainerTracking;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-jwt-key-min-32-characters-long",
    "Issuer": "MaerskContainerTracking",
    "Audience": "MaerskContainerTracking",
    "ExpiryMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### 5. Database Migration & Seeding

```bash
# Create initial migration (if not exists)
dotnet ef migrations add InitialMigration

# Apply migrations to database
dotnet ef database update

# The application will automatically seed data on first run
```

### 6. Run the Application

```bash
# Start the development server
dotnet run

# Or with file watching (auto-restart on changes)
dotnet watch run

# Application will be available at:
# - API: http://localhost:5221
# - Swagger UI: http://localhost:5221/swagger
```

## 🔧 Development Tools Setup

### VS Code Extensions (Recommended)
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "ms-vscode.vscode-json",
    "ms-azuretools.vscode-docker",
    "humao.rest-client",
    "bradlc.vscode-tailwindcss"
  ]
}
```

### VS Code Settings
Create `.vscode/settings.json`:
```json
{
  "dotnet.defaultSolution": "backend.sln",
  "files.exclude": {
    "**/bin": true,
    "**/obj": true
  },
  "omnisharp.enableRoslynAnalyzers": true,
  "omnisharp.useModernNet": true
}
```

### VS Code Launch Configuration
Create `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net8.0/backend.dll",
      "args": [],
      "cwd": "${workspaceFolder}",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

### VS Code Tasks Configuration
Create `.vscode/tasks.json`:
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": ["build", "${workspaceFolder}/backend.csproj"],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "run",
      "command": "dotnet",
      "type": "process",
      "args": ["run", "--project", "${workspaceFolder}/backend.csproj"],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

## 🧪 Development Workflow

### 1. **Daily Development Routine**
```bash
# 1. Pull latest changes
git pull origin main

# 2. Restore packages (if packages changed)
dotnet restore

# 3. Apply any new migrations
dotnet ef database update

# 4. Start development server
dotnet watch run

# 5. Open Swagger UI for testing
# http://localhost:5221/swagger
```

### 2. **Making Code Changes**

#### Controller Development:
```bash
# Add new controller
mkdir Controllers
touch Controllers/NewFeatureController.cs

# Add corresponding service
mkdir Services
touch Services/NewFeatureService.cs

# Add repository
mkdir Repositories
touch Repositories/NewFeatureRepository.cs
```

#### Database Changes:
```bash
# Add new entity model
touch Models/NewEntity.cs

# Create migration
dotnet ef migrations add AddNewEntity

# Apply migration
dotnet ef database update

# Rollback if needed
dotnet ef database update PreviousMigration
```

### 3. **Testing Workflow**
```bash
# Run unit tests
dotnet test

# Run specific test project
dotnet test Tests/UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Test API endpoints with curl
curl -X GET http://localhost:5221/api/health/live
```

## 🔍 Debugging & Troubleshooting

### Common Issues & Solutions

#### 1. **Database Connection Issues**
```bash
# Test PostgreSQL connection
psql -h localhost -p 5433 -U postgres -d ContainerTracking

# Check if PostgreSQL is running
docker ps | grep postgres

# Restart PostgreSQL container
docker restart postgres-container
```

#### 2. **Migration Issues**
```bash
# Reset database (development only)
dotnet ef database drop --force
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list
```

#### 3. **Port Conflicts**
```bash
# Check what's running on port 5221
netstat -ano | findstr :5221

# Kill process on Windows
taskkill /PID <process_id> /F

# Change port in launchSettings.json
```

#### 4. **NuGet Package Issues**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages with verbose output
dotnet restore --verbosity normal

# Update all packages
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
```

## 🏗️ Project Structure

```
backend/
├── Controllers/              # API endpoints
├── Services/                # Business logic
├── Repositories/            # Data access
├── Models/                  # Entity models
├── DTOs/                   # Data transfer objects
├── Middleware/             # Custom middleware
├── Extensions/             # Service extensions
├── Migrations/             # EF Core migrations
├── Constants/              # Application constants
├── Attributes/             # Custom attributes
├── Data/                   # DbContext
├── Properties/             # Assembly info
├── bin/                    # Compiled output
├── obj/                    # Build artifacts
├── backend.csproj          # Project file
├── Program.cs              # Application entry point
├── appsettings.json        # Configuration
├── appsettings.Development.json
└── .env                    # Environment variables
```

## 🔒 Security Configuration

### 1. **JWT Configuration**
```csharp
// In Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
        };
    });
```

### 2. **CORS Configuration**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
```

## 📊 Performance Optimization

### 1. **Database Performance**
```csharp
// Connection pooling in Program.cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.SetPostgresVersion(14, 0))
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .LogTo(Console.WriteLine, LogLevel.Information));
```

### 2. **API Response Caching**
```csharp
// Add response caching
builder.Services.AddResponseCaching();

// Use in controllers
[ResponseCache(Duration = 300)]
public async Task<IActionResult> GetPorts()
```

## 🚀 Deployment Preparation

### 1. **Production Configuration**
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=prod-db-server;Database=ContainerTracking;Username=prod_user;Password=secure_password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 2. **Docker Support**
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend.csproj", "."]
RUN dotnet restore "./backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend.dll"]
```

## 📚 Additional Resources

- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [JWT Authentication](https://jwt.io/introduction/)

## 🆘 Getting Help

If you encounter issues:

1. **Check Logs**: Review console output and log files
2. **Database Connection**: Verify PostgreSQL is running and accessible
3. **Environment Variables**: Ensure all required settings are configured
4. **Package Versions**: Verify compatible package versions
5. **Documentation**: Refer to this guide and official documentation
6. **Team Support**: Reach out to team members or create GitHub issues

## ✅ Verification Checklist

- [ ] .NET 8 SDK installed
- [ ] PostgreSQL running (local or Docker)
- [ ] Dependencies restored (`dotnet restore`)
- [ ] Environment variables configured
- [ ] Database migrations applied
- [ ] Application starts without errors
- [ ] Swagger UI accessible at `http://localhost:5221/swagger`
- [ ] Can authenticate with default admin credentials
- [ ] API endpoints respond correctly