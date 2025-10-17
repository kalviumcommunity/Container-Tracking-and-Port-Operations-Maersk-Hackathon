# 🚀 Backend Deployment Guide

## Deployment Overview

This guide covers deploying the Maersk Container Tracking backend API to various environments including Azure, AWS, Docker containers, and traditional hosting platforms.

## 🎯 Deployment Options

### 1. **Azure App Service (Recommended)**
- Managed platform with auto-scaling
- Built-in CI/CD integration
- Azure Database for PostgreSQL integration
- SSL certificates and custom domains

### 2. **Docker Containers**
- Consistent environments across development/production
- Easy scaling with container orchestration
- Works with Azure Container Instances, AWS ECS, Kubernetes

### 3. **Traditional Hosting (IIS/Linux)**
- On-premises or VPS deployment
- Full control over server configuration
- Manual scaling and maintenance

## 🔷 Azure App Service Deployment

### Prerequisites
- Azure subscription
- Azure CLI installed
- .NET 8 SDK

### 1. **Azure Resources Setup**

#### Create Resource Group:
```bash
# Login to Azure
az login

# Create resource group
az group create --name "rg-container-tracking" --location "West Europe"
```

#### Create PostgreSQL Server:
```bash
# Create PostgreSQL Flexible Server
az postgres flexible-server create \
  --resource-group "rg-container-tracking" \
  --name "pg-container-tracking" \
  --location "West Europe" \
  --admin-user "adminuser" \
  --admin-password "SecurePassword123!" \
  --version "14" \
  --sku-name "Standard_B1ms"

# Create database
az postgres flexible-server db create \
  --resource-group "rg-container-tracking" \
  --server-name "pg-container-tracking" \
  --database-name "ContainerTracking"

# Configure firewall (allow Azure services)
az postgres flexible-server firewall-rule create \
  --resource-group "rg-container-tracking" \
  --name "pg-container-tracking" \
  --rule-name "AllowAzureServices" \
  --start-ip-address "0.0.0.0" \
  --end-ip-address "0.0.0.0"
```

#### Create App Service:
```bash
# Create App Service Plan
az appservice plan create \
  --name "plan-container-tracking" \
  --resource-group "rg-container-tracking" \
  --sku "S1" \
  --is-linux

# Create Web App
az webapp create \
  --resource-group "rg-container-tracking" \
  --plan "plan-container-tracking" \
  --name "api-container-tracking" \
  --runtime "DOTNETCORE:8.0"
```

### 2. **Application Configuration**

#### Configure Connection String:
```bash
# Set connection string
az webapp config connection-string set \
  --resource-group "rg-container-tracking" \
  --name "api-container-tracking" \
  --connection-string-type "PostgreSQL" \
  --settings "DefaultConnection=Host=pg-container-tracking.postgres.database.azure.com;Database=ContainerTracking;Username=adminuser;Password=SecurePassword123!"
```

#### Configure App Settings:
```bash
# Set application settings
az webapp config appsettings set \
  --resource-group "rg-container-tracking" \
  --name "api-container-tracking" \
  --settings \
    "JWT__SecretKey=your-production-jwt-secret-key-min-32-characters" \
    "JWT__Issuer=MaerskContainerTracking" \
    "JWT__Audience=MaerskContainerTracking" \
    "JWT__ExpiryMinutes=1440" \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "CORS__AllowedOrigins=https://your-frontend-domain.com"
```

### 3. **GitHub Actions Deployment**

#### Create `.github/workflows/deploy-backend.yml`:
```yaml
name: Deploy Backend to Azure

on:
  push:
    branches: [ main ]
    paths: [ 'backend/**' ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore ./backend/backend.csproj
    
    - name: Build application
      run: dotnet build ./backend/backend.csproj --no-restore --configuration Release
    
    - name: Run tests
      run: dotnet test ./backend --no-build --configuration Release
    
    - name: Publish application
      run: dotnet publish ./backend/backend.csproj --no-build --configuration Release --output ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'api-container-tracking'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

#### Setup Deployment Secrets:
1. Download publish profile from Azure Portal
2. Add `AZURE_WEBAPP_PUBLISH_PROFILE` to GitHub Secrets

### 4. **Database Migration**

#### Run Migrations on Deployment:
```csharp
// In Program.cs
if (app.Environment.IsProduction())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}
```

#### Manual Migration:
```bash
# Connect to production database and run migrations
dotnet ef database update --connection "Host=pg-container-tracking.postgres.database.azure.com;Database=ContainerTracking;Username=adminuser;Password=SecurePassword123!"
```

## 🐳 Docker Deployment

### 1. **Create Dockerfile**

#### `Dockerfile`:
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["backend.csproj", "."]
RUN dotnet restore "backend.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/api/health/live || exit 1

ENTRYPOINT ["dotnet", "backend.dll"]
```

### 2. **Docker Compose for Full Stack**

#### `docker-compose.production.yml`:
```yaml
version: '3.8'

services:
  postgres:
    image: postgres:14
    environment:
      POSTGRES_DB: ContainerTracking
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 30s
      timeout: 10s
      retries: 5

  api:
    build:
      context: ./backend
      dockerfile: Dockerfile
    environment:
      ConnectionStrings__DefaultConnection: "Host=postgres;Database=ContainerTracking;Username=postgres;Password=${POSTGRES_PASSWORD}"
      JWT__SecretKey: ${JWT_SECRET_KEY}
      JWT__Issuer: "MaerskContainerTracking"
      JWT__Audience: "MaerskContainerTracking"
      CORS__AllowedOrigins: "https://your-frontend-domain.com"
    ports:
      - "8080:8080"
    depends_on:
      postgres:
        condition: service_healthy
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/api/health/live"]
      interval: 30s
      timeout: 10s
      retries: 3

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/ssl:/etc/nginx/ssl
    depends_on:
      - api

volumes:
  postgres_data:
```

#### `.env` file for Docker:
```env
POSTGRES_PASSWORD=SecureProductionPassword123!
JWT_SECRET_KEY=your-production-jwt-secret-key-minimum-32-characters-long
```

### 3. **Build and Deploy Docker**

```bash
# Build image
docker build -t container-tracking-api ./backend

# Run with environment variables
docker run -d \
  --name container-tracking-api \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=your-db-host;Database=ContainerTracking;Username=user;Password=pass" \
  -e JWT__SecretKey="your-jwt-secret" \
  container-tracking-api

# Or use docker-compose
docker-compose -f docker-compose.production.yml up -d
```

## ☁️ AWS Deployment

### 1. **AWS ECS Deployment**

#### Create ECS Task Definition:
```json
{
  "family": "container-tracking-api",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "executionRoleArn": "arn:aws:iam::account:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "api",
      "image": "your-account.dkr.ecr.region.amazonaws.com/container-tracking-api:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "secrets": [
        {
          "name": "ConnectionStrings__DefaultConnection",
          "valueFrom": "arn:aws:secretsmanager:region:account:secret:prod/containertracking/db"
        },
        {
          "name": "JWT__SecretKey",
          "valueFrom": "arn:aws:secretsmanager:region:account:secret:prod/containertracking/jwt"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/container-tracking-api",
          "awslogs-region": "us-west-2",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

### 2. **AWS RDS PostgreSQL**

```bash
# Create RDS instance
aws rds create-db-instance \
  --db-instance-identifier "container-tracking-db" \
  --db-instance-class "db.t3.micro" \
  --engine "postgres" \
  --engine-version "14.9" \
  --master-username "adminuser" \
  --master-user-password "SecurePassword123!" \
  --allocated-storage 20 \
  --vpc-security-group-ids "sg-xxxxxx" \
  --db-subnet-group-name "default"
```

## 🔧 Production Configuration

### 1. **Production appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD};SSL Mode=Require"
  },
  "Jwt": {
    "SecretKey": "${JWT_SECRET_KEY}",
    "Issuer": "MaerskContainerTracking",
    "Audience": "MaerskContainerTracking",
    "ExpiryMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  },
  "CORS": {
    "AllowedOrigins": ["https://your-frontend-domain.com"]
  },
  "ApplicationInsights": {
    "InstrumentationKey": "${APPINSIGHTS_INSTRUMENTATIONKEY}"
  }
}
```

### 2. **Environment Variables**

#### Required Production Environment Variables:
```bash
# Database
DB_HOST=your-database-host
DB_NAME=ContainerTracking
DB_USER=your-username
DB_PASSWORD=your-secure-password

# JWT Configuration
JWT_SECRET_KEY=your-production-jwt-secret-minimum-32-characters
JWT_ISSUER=MaerskContainerTracking
JWT_AUDIENCE=MaerskContainerTracking

# Application
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# CORS
CORS_ALLOWED_ORIGINS=https://your-frontend-domain.com

# Monitoring
APPINSIGHTS_INSTRUMENTATIONKEY=your-app-insights-key
```

## 🔒 Production Security Checklist

### SSL/TLS Configuration:
- [ ] Enable HTTPS only
- [ ] Configure SSL certificates
- [ ] Implement HSTS headers
- [ ] Set secure cookie policies

### Database Security:
- [ ] Use connection string encryption
- [ ] Enable SSL for database connections
- [ ] Implement database firewall rules
- [ ] Regular database backups

### Application Security:
- [ ] Secure JWT secret keys
- [ ] Configure CORS properly
- [ ] Implement rate limiting
- [ ] Enable security headers
- [ ] Validate all inputs

### Infrastructure Security:
- [ ] Network security groups
- [ ] VPN or private network access
- [ ] Regular security updates
- [ ] Monitoring and alerting

## 📊 Monitoring & Logging

### 1. **Application Insights (Azure)**

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();

// Custom telemetry
public class ContainerService
{
    private readonly TelemetryClient _telemetryClient;
    
    public async Task<Container> CreateAsync(Container container)
    {
        _telemetryClient.TrackEvent("ContainerCreated", 
            new Dictionary<string, string> 
            {
                ["ContainerNumber"] = container.ContainerNumber,
                ["Type"] = container.Type
            });
    }
}
```

### 2. **Health Checks**

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "database")
    .AddCheck<ApiHealthCheck>("api");

app.MapHealthChecks("/api/health/live");
app.MapHealthChecks("/api/health/ready");
```

### 3. **Structured Logging**

```csharp
// Program.cs
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces)
);
```

## 🔄 CI/CD Pipeline

### Complete GitHub Actions Workflow:

```yaml
name: Deploy to Production

on:
  push:
    branches: [ main ]

env:
  AZURE_WEBAPP_NAME: api-container-tracking
  DOCKER_IMAGE_NAME: container-tracking-api

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Restore dependencies
      run: dotnet restore ./backend
    - name: Run tests
      run: dotnet test ./backend

  build-and-deploy:
    needs: test
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Publish application
      run: |
        dotnet publish ./backend/backend.csproj \
          --configuration Release \
          --output ./publish \
          --self-contained false
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: ${{ env.AZURE_WEBAPP_NAME }}
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
    
    - name: Run Database Migrations
      run: |
        dotnet tool install --global dotnet-ef
        dotnet ef database update --project ./backend \
          --connection "${{ secrets.PRODUCTION_CONNECTION_STRING }}"
```

## 🚀 Post-Deployment Verification

### 1. **Health Check Verification**
```bash
# Test health endpoints
curl -f https://api-container-tracking.azurewebsites.net/api/health/live
curl -f https://api-container-tracking.azurewebsites.net/api/health/ready

# Test authentication
curl -X POST https://api-container-tracking.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```

### 2. **Performance Testing**
```bash
# Load testing with artillery
artillery run production-load-test.yml

# Monitor response times
curl -w "@curl-format.txt" -o /dev/null -s https://your-api-domain/api/containers
```

### 3. **Monitoring Setup**
- Configure application insights dashboards
- Set up alerts for critical errors
- Monitor database performance
- Track API response times

This comprehensive deployment guide ensures your Maersk Container Tracking backend API is production-ready, secure, and scalable across different cloud platforms and hosting environments.