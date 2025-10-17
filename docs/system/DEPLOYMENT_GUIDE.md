# 🚀 Deployment Guide

## Overview

This comprehensive deployment guide covers all aspects of deploying the Maersk Container Tracking & Port Operations System to various environments, from development to production. The system supports multiple deployment strategies including traditional cloud deployment, containerized deployment, and hybrid approaches.

## 🏗️ Deployment Architecture

### System Components
- **Frontend**: Vue.js 3 Single Page Application
- **Backend**: .NET 8 Web API
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis (optional but recommended)
- **Message Queue**: Kafka for event streaming
- **File Storage**: Azure Blob Storage or AWS S3
- **Monitoring**: Application Insights or equivalent

### Supported Deployment Platforms
- **Azure**: App Service, Container Instances, AKS
- **AWS**: Elastic Beanstalk, ECS, EKS
- **Google Cloud**: App Engine, Cloud Run, GKE
- **On-Premises**: Docker, Kubernetes
- **Hybrid**: Multi-cloud or cloud-on-premises

## 🔧 Prerequisites

### Required Software
- **Docker**: Latest version
- **Kubernetes CLI** (if using K8s): kubectl
- **Cloud CLI Tools**: Azure CLI, AWS CLI, or gcloud
- **.NET SDK**: 8.0 or higher
- **Node.js**: 20.x LTS
- **PostgreSQL**: 14.x or higher

### Required Accounts & Credentials
- Cloud provider account (Azure/AWS/GCP)
- Domain name and SSL certificates
- Email service credentials (SendGrid, AWS SES)
- External API keys (if applicable)

## 🌍 Environment Configuration

### Environment Types
| Environment | Purpose | URL Pattern | Database |
|-------------|---------|-------------|----------|
| **Development** | Local development | localhost | Local PostgreSQL |
| **Testing** | Automated testing | test.domain.com | Test database |
| **Staging** | Pre-production validation | staging.domain.com | Staging database |
| **Production** | Live system | domain.com | Production database |

### Environment Variables

#### Backend Configuration
```bash
# Database Configuration
CONNECTION_STRING="Host=localhost;Database=container_tracking;Username=postgres;Password=your_password"

# JWT Configuration
JWT_KEY="your-256-bit-secret-key-here"
JWT_ISSUER="ContainerTracking"
JWT_AUDIENCE="ContainerTrackingUsers"
JWT_DURATION_MINUTES="60"

# External Services
REDIS_CONNECTION_STRING="localhost:6379"
KAFKA_BOOTSTRAP_SERVERS="localhost:9092"
BLOB_STORAGE_CONNECTION_STRING="your-storage-connection-string"

# Email Configuration
SENDGRID_API_KEY="your-sendgrid-api-key"
EMAIL_FROM="noreply@containertracking.com"

# Application Settings
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="https://+:443;http://+:80"
ALLOWED_ORIGINS="https://containertracking.com,https://www.containertracking.com"
```

#### Frontend Configuration
```bash
# API Configuration
VITE_API_BASE_URL="https://api.containertracking.com"
VITE_API_TIMEOUT="10000"

# Application Configuration
VITE_APP_TITLE="Container Tracking System"
VITE_APP_VERSION="1.0.0"
VITE_ENVIRONMENT="production"

# Feature Flags
VITE_ENABLE_ANALYTICS="true"
VITE_ENABLE_NOTIFICATIONS="true"
VITE_DEBUG_MODE="false"

# External Services
VITE_GOOGLE_ANALYTICS_ID="GA_TRACKING_ID"
```

## 🐳 Docker Deployment

### Docker Images

#### Backend Dockerfile
```dockerfile
# Backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/backend.csproj", "backend/"]
RUN dotnet restore "backend/backend.csproj"
COPY . .
WORKDIR "/src/backend"
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "backend.dll"]
```

#### Frontend Dockerfile
```dockerfile
# Frontend Dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY frontend/package*.json ./
RUN npm ci --only=production
COPY frontend/ .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY frontend/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### Docker Compose Configuration

#### Development Environment
```yaml
# docker-compose.dev.yml
version: '3.8'
services:
  postgres:
    image: postgres:14
    environment:
      POSTGRES_DB: container_tracking_dev
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: dev_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:alpine
    ports:
      - "6379:6379"

  backend:
    build:
      context: .
      dockerfile: backend/Dockerfile
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=container_tracking_dev;Username=postgres;Password=dev_password
      - Redis__ConnectionString=redis:6379
    ports:
      - "5266:80"
    depends_on:
      - postgres
      - redis

  frontend:
    build:
      context: .
      dockerfile: frontend/Dockerfile
    ports:
      - "5173:80"
    depends_on:
      - backend

volumes:
  postgres_data:
```

#### Production Environment
```yaml
# docker-compose.prod.yml
version: '3.8'
services:
  backend:
    image: containertracking/backend:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=${CONNECTION_STRING}
      - JwtSettings__Key=${JWT_KEY}
      - Redis__ConnectionString=${REDIS_CONNECTION_STRING}
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./certificates:/https:ro
    restart: unless-stopped

  frontend:
    image: containertracking/frontend:latest
    ports:
      - "8080:80"
    restart: unless-stopped

  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./certificates:/etc/nginx/certificates:ro
    depends_on:
      - backend
      - frontend
    restart: unless-stopped
```

### Building and Running Docker Containers

#### Build Images
```bash
# Build backend image
docker build -f backend/Dockerfile -t containertracking/backend:latest .

# Build frontend image
docker build -f frontend/Dockerfile -t containertracking/frontend:latest .

# Build all services
docker-compose -f docker-compose.prod.yml build
```

#### Run Containers
```bash
# Development environment
docker-compose -f docker-compose.dev.yml up -d

# Production environment
docker-compose -f docker-compose.prod.yml up -d

# Check container status
docker-compose ps

# View logs
docker-compose logs -f backend
docker-compose logs -f frontend
```

## ☁️ Azure Deployment

### Azure App Service Deployment

#### Resource Creation
```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-containertracking --location "East US"

# Create App Service plan
az appservice plan create \
  --name asp-containertracking \
  --resource-group rg-containertracking \
  --sku B1 \
  --is-linux

# Create backend app service
az webapp create \
  --resource-group rg-containertracking \
  --plan asp-containertracking \
  --name api-containertracking \
  --runtime "DOTNETCORE:8.0"

# Create frontend app service (Static Web App)
az staticwebapp create \
  --name containertracking-frontend \
  --resource-group rg-containertracking \
  --source https://github.com/yourusername/container-tracking \
  --branch main \
  --app-location "frontend" \
  --api-location "backend" \
  --output-location "dist"
```

#### Database Setup
```bash
# Create PostgreSQL server
az postgres flexible-server create \
  --resource-group rg-containertracking \
  --name postgres-containertracking \
  --location "East US" \
  --admin-user adminuser \
  --admin-password "YourSecurePassword123!" \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --public-access 0.0.0.0 \
  --storage-size 32

# Create database
az postgres flexible-server db create \
  --resource-group rg-containertracking \
  --server-name postgres-containertracking \
  --database-name container_tracking_prod
```

#### Configuration
```bash
# Set backend app settings
az webapp config appsettings set \
  --resource-group rg-containertracking \
  --name api-containertracking \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    ConnectionStrings__DefaultConnection="Host=postgres-containertracking.postgres.database.azure.com;Database=container_tracking_prod;Username=adminuser;Password=YourSecurePassword123!" \
    JwtSettings__Key="your-super-secret-jwt-key-minimum-256-bits-for-production" \
    AllowedOrigins="https://containertracking-frontend.azurestaticapps.net"

# Enable HTTPS only
az webapp update \
  --resource-group rg-containertracking \
  --name api-containertracking \
  --https-only true
```

### Azure Container Instances

#### Container Group Deployment
```yaml
# azure-container-group.yml
apiVersion: 2019-12-01
location: eastus
name: containertracking-group
properties:
  containers:
  - name: backend
    properties:
      image: containertracking/backend:latest
      resources:
        requests:
          cpu: 1
          memoryInGb: 1.5
      ports:
      - protocol: tcp
        port: 80
      environmentVariables:
      - name: ASPNETCORE_ENVIRONMENT
        value: Production
      - name: ConnectionStrings__DefaultConnection
        secureValue: "Host=your-postgres-server;Database=container_tracking;Username=user;Password=password"
  
  - name: frontend
    properties:
      image: containertracking/frontend:latest
      resources:
        requests:
          cpu: 0.5
          memoryInGb: 0.5
      ports:
      - protocol: tcp
        port: 80

  osType: Linux
  ipAddress:
    type: Public
    ports:
    - protocol: tcp
      port: 80
    - protocol: tcp
      port: 443
    dnsNameLabel: containertracking-prod
  restartPolicy: Always
```

```bash
# Deploy container group
az container create \
  --resource-group rg-containertracking \
  --file azure-container-group.yml
```

## 🔧 Kubernetes Deployment

### Kubernetes Manifests

#### Namespace
```yaml
# k8s/namespace.yml
apiVersion: v1
kind: Namespace
metadata:
  name: containertracking
```

#### ConfigMap
```yaml
# k8s/configmap.yml
apiVersion: v1
kind: ConfigMap
metadata:
  name: app-config
  namespace: containertracking
data:
  ASPNETCORE_ENVIRONMENT: "Production"
  JWT_ISSUER: "ContainerTracking"
  JWT_AUDIENCE: "ContainerTrackingUsers"
  REDIS_HOST: "redis-service"
  REDIS_PORT: "6379"
```

#### Secrets
```yaml
# k8s/secrets.yml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
  namespace: containertracking
type: Opaque
data:
  CONNECTION_STRING: <base64-encoded-connection-string>
  JWT_KEY: <base64-encoded-jwt-key>
  SENDGRID_API_KEY: <base64-encoded-api-key>
```

#### Backend Deployment
```yaml
# k8s/backend-deployment.yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: backend-deployment
  namespace: containertracking
spec:
  replicas: 3
  selector:
    matchLabels:
      app: backend
  template:
    metadata:
      labels:
        app: backend
    spec:
      containers:
      - name: backend
        image: containertracking/backend:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          valueFrom:
            configMapKeyRef:
              name: app-config
              key: ASPNETCORE_ENVIRONMENT
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: CONNECTION_STRING
        - name: JwtSettings__Key
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: JWT_KEY
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /api/health/live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /api/health/ready
            port: 80
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: backend-service
  namespace: containertracking
spec:
  selector:
    app: backend
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
  type: ClusterIP
```

#### Frontend Deployment
```yaml
# k8s/frontend-deployment.yml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: frontend-deployment
  namespace: containertracking
spec:
  replicas: 2
  selector:
    matchLabels:
      app: frontend
  template:
    metadata:
      labels:
        app: frontend
    spec:
      containers:
      - name: frontend
        image: containertracking/frontend:latest
        ports:
        - containerPort: 80
        resources:
          requests:
            memory: "128Mi"
            cpu: "100m"
          limits:
            memory: "256Mi"
            cpu: "200m"
---
apiVersion: v1
kind: Service
metadata:
  name: frontend-service
  namespace: containertracking
spec:
  selector:
    app: frontend
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
  type: ClusterIP
```

#### Ingress Configuration
```yaml
# k8s/ingress.yml
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: containertracking-ingress
  namespace: containertracking
  annotations:
    kubernetes.io/ingress.class: nginx
    cert-manager.io/cluster-issuer: letsencrypt-prod
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
spec:
  tls:
  - hosts:
    - containertracking.com
    - api.containertracking.com
    secretName: containertracking-tls
  rules:
  - host: containertracking.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: frontend-service
            port:
              number: 80
  - host: api.containertracking.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: backend-service
            port:
              number: 80
```

### Deployment Commands
```bash
# Apply all manifests
kubectl apply -f k8s/namespace.yml
kubectl apply -f k8s/configmap.yml
kubectl apply -f k8s/secrets.yml
kubectl apply -f k8s/backend-deployment.yml
kubectl apply -f k8s/frontend-deployment.yml
kubectl apply -f k8s/ingress.yml

# Check deployment status
kubectl get pods -n containertracking
kubectl get services -n containertracking
kubectl get ingress -n containertracking

# View logs
kubectl logs -f deployment/backend-deployment -n containertracking
kubectl logs -f deployment/frontend-deployment -n containertracking
```

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

#### Main Deployment Pipeline
```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]
  workflow_dispatch:

env:
  REGISTRY: ghcr.io
  IMAGE_NAME: ${{ github.repository }}

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '20'
    
    - name: Run backend tests
      run: |
        cd backend
        dotnet restore
        dotnet test --no-restore --verbosity normal
    
    - name: Run frontend tests
      run: |
        cd frontend
        npm ci
        npm run test:unit

  build-and-push:
    needs: test
    runs-on: ubuntu-latest
    permissions:
      contents: read
      packages: write
    
    steps:
    - name: Checkout repository
      uses: actions/checkout@v4
    
    - name: Log in to Container Registry
      uses: docker/login-action@v2
      with:
        registry: ${{ env.REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Build and push Backend image
      uses: docker/build-push-action@v4
      with:
        context: .
        file: ./backend/Dockerfile
        push: true
        tags: |
          ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/backend:latest
          ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/backend:${{ github.sha }}
    
    - name: Build and push Frontend image
      uses: docker/build-push-action@v4
      with:
        context: .
        file: ./frontend/Dockerfile
        push: true
        tags: |
          ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/frontend:latest
          ${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/frontend:${{ github.sha }}

  deploy-azure:
    needs: build-and-push
    runs-on: ubuntu-latest
    environment: production
    
    steps:
    - name: Deploy to Azure App Service
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'api-containertracking'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        images: '${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/backend:${{ github.sha }}'

  deploy-k8s:
    needs: build-and-push
    runs-on: ubuntu-latest
    environment: production
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up kubectl
      uses: azure/setup-kubectl@v3
      with:
        version: 'v1.24.0'
    
    - name: Deploy to Kubernetes
      run: |
        # Update image tags in k8s manifests
        sed -i 's|containertracking/backend:latest|${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/backend:${{ github.sha }}|' k8s/backend-deployment.yml
        sed -i 's|containertracking/frontend:latest|${{ env.REGISTRY }}/${{ env.IMAGE_NAME }}/frontend:${{ github.sha }}|' k8s/frontend-deployment.yml
        
        # Apply manifests
        kubectl apply -f k8s/
        
        # Wait for rollout
        kubectl rollout status deployment/backend-deployment -n containertracking
        kubectl rollout status deployment/frontend-deployment -n containertracking
      env:
        KUBE_CONFIG: ${{ secrets.KUBE_CONFIG }}
```

### Azure DevOps Pipeline
```yaml
# azure-pipelines.yml
trigger:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  containerRegistry: 'containertrackingregistry.azurecr.io'

stages:
- stage: Test
  displayName: 'Test Stage'
  jobs:
  - job: RunTests
    displayName: 'Run Tests'
    steps:
    - task: UseDotNet@2
      displayName: 'Use .NET 8 SDK'
      inputs:
        packageType: 'sdk'
        version: '8.0.x'
    
    - task: NodeTool@0
      displayName: 'Use Node.js 20'
      inputs:
        versionSpec: '20.x'
    
    - script: |
        cd backend
        dotnet restore
        dotnet test --configuration $(buildConfiguration) --logger trx --collect:"XPlat Code Coverage"
      displayName: 'Run Backend Tests'
    
    - script: |
        cd frontend
        npm ci
        npm run test:unit -- --coverage
      displayName: 'Run Frontend Tests'

- stage: Build
  displayName: 'Build and Push Images'
  dependsOn: Test
  condition: succeeded()
  jobs:
  - job: BuildImages
    displayName: 'Build Docker Images'
    steps:
    - task: Docker@2
      displayName: 'Build and push backend image'
      inputs:
        containerRegistry: 'Container Registry Connection'
        repository: 'containertracking/backend'
        command: 'buildAndPush'
        Dockerfile: 'backend/Dockerfile'
        tags: |
          latest
          $(Build.BuildNumber)
    
    - task: Docker@2
      displayName: 'Build and push frontend image'
      inputs:
        containerRegistry: 'Container Registry Connection'
        repository: 'containertracking/frontend'
        command: 'buildAndPush'
        Dockerfile: 'frontend/Dockerfile'
        tags: |
          latest
          $(Build.BuildNumber)

- stage: Deploy
  displayName: 'Deploy to Production'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  jobs:
  - deployment: DeployToProduction
    displayName: 'Deploy to Azure'
    environment: 'production'
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            displayName: 'Deploy Backend to Azure App Service'
            inputs:
              azureSubscription: 'Azure Service Connection'
              appType: 'webAppContainer'
              appName: 'api-containertracking'
              deployToSlotOrASE: true
              resourceGroupName: 'rg-containertracking'
              slotName: 'production'
              imageName: '$(containerRegistry)/containertracking/backend:$(Build.BuildNumber)'
```

## 📊 Monitoring & Health Checks

### Application Health Checks

#### Backend Health Endpoints
```csharp
// Program.cs - Health checks configuration
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddRedis(builder.Configuration.GetConnectionString("Redis"))
    .AddUrlGroup(new Uri("https://external-api.com/health"), "External API");

app.MapHealthChecks("/api/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

#### Frontend Health Monitoring
```typescript
// src/services/health.service.ts
export class HealthService {
  async checkBackendHealth(): Promise<boolean> {
    try {
      const response = await axios.get('/api/health/ready', {
        timeout: 5000
      })
      return response.status === 200
    } catch (error) {
      console.error('Backend health check failed:', error)
      return false
    }
  }
  
  async performHealthCheck(): Promise<HealthStatus> {
    return {
      frontend: true,
      backend: await this.checkBackendHealth(),
      timestamp: new Date().toISOString()
    }
  }
}
```

### Logging Configuration

#### Backend Logging (Serilog)
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
    .CreateLogger();

builder.Host.UseSerilog();
```

#### Application Insights Configuration
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=your-key;IngestionEndpoint=https://your-region.in.applicationinsights.azure.com/",
    "EnableAdaptiveSampling": false,
    "EnablePerformanceCounterCollectionModule": true,
    "EnableRequestTrackingTelemetryModule": true,
    "EnableEventCounterCollectionModule": true,
    "EnableDependencyTrackingTelemetryModule": true,
    "EnableAppServicesHeartbeatTelemetryModule": true,
    "EnableAzureInstanceMetadataTelemetryModule": true
  }
}
```

## 🔧 Database Migration & Seeding

### Production Database Setup

#### Migration Commands
```bash
# Generate migration
cd backend
dotnet ef migrations add ProductionMigration

# Update database
dotnet ef database update --connection "Host=your-prod-server;Database=container_tracking_prod;Username=user;Password=password"

# Generate SQL script for manual execution
dotnet ef migrations script --output migration.sql
```

#### Data Seeding Strategy
```csharp
// Data/SeedData.cs
public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Ports.Any())
        {
            await SeedPorts(context);
        }
        
        if (!context.Users.Any())
        {
            await SeedDefaultUsers(context);
        }
        
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedPorts(ApplicationDbContext context)
    {
        var ports = new List<Port>
        {
            new Port { Name = "Port of Los Angeles", Code = "USLAX", Country = "United States", City = "Los Angeles" },
            new Port { Name = "Port of Rotterdam", Code = "NLRTM", Country = "Netherlands", City = "Rotterdam" },
            // Additional ports...
        };
        
        context.Ports.AddRange(ports);
    }
}
```

## 🔒 Security Configuration

### SSL/TLS Configuration

#### Certificate Management
```bash
# Using Let's Encrypt with Certbot
certbot --nginx -d containertracking.com -d www.containertracking.com -d api.containertracking.com

# Manual certificate installation
sudo cp your-certificate.crt /etc/ssl/certs/
sudo cp your-private.key /etc/ssl/private/
sudo chmod 600 /etc/ssl/private/your-private.key
```

#### Nginx SSL Configuration
```nginx
# /etc/nginx/sites-available/containertracking
server {
    listen 443 ssl http2;
    server_name containertracking.com www.containertracking.com;
    
    ssl_certificate /etc/ssl/certs/containertracking.crt;
    ssl_certificate_key /etc/ssl/private/containertracking.key;
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512;
    ssl_prefer_server_ciphers off;
    
    # Security headers
    add_header Strict-Transport-Security "max-age=63072000" always;
    add_header X-Content-Type-Options nosniff;
    add_header X-Frame-Options DENY;
    add_header X-XSS-Protection "1; mode=block";
    add_header Referrer-Policy "strict-origin-when-cross-origin";
    
    location / {
        proxy_pass http://localhost:8080;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

server {
    listen 443 ssl http2;
    server_name api.containertracking.com;
    
    ssl_certificate /etc/ssl/certs/containertracking.crt;
    ssl_certificate_key /etc/ssl/private/containertracking.key;
    
    location / {
        proxy_pass http://localhost:5266;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## 🚨 Backup & Disaster Recovery

### Database Backup

#### Automated Backup Script
```bash
#!/bin/bash
# backup-database.sh

# Configuration
DB_HOST="your-postgres-server"
DB_NAME="container_tracking_prod"
DB_USER="postgres"
BACKUP_DIR="/backups/postgres"
DATE=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="$BACKUP_DIR/container_tracking_backup_$DATE.sql"

# Create backup directory
mkdir -p $BACKUP_DIR

# Create database backup
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME > $BACKUP_FILE

# Compress backup
gzip $BACKUP_FILE

# Upload to cloud storage (optional)
aws s3 cp "$BACKUP_FILE.gz" s3://your-backup-bucket/database/

# Clean up old backups (keep last 30 days)
find $BACKUP_DIR -name "*.gz" -mtime +30 -delete

echo "Backup completed: $BACKUP_FILE.gz"
```

#### Backup Restoration
```bash
# Restore from backup
gunzip container_tracking_backup_20241216_120000.sql.gz
psql -h $DB_HOST -U $DB_USER -d $DB_NAME < container_tracking_backup_20241216_120000.sql
```

### Application Recovery

#### Container Recovery
```bash
# Check container status
docker ps -a

# Restart failed containers
docker-compose restart backend frontend

# Restore from backup images
docker pull containertracking/backend:stable
docker pull containertracking/frontend:stable

# Rollback deployment
docker-compose down
docker-compose up -d
```

#### Kubernetes Recovery
```bash
# Check pod status
kubectl get pods -n containertracking

# Restart failed deployments
kubectl rollout restart deployment/backend-deployment -n containertracking
kubectl rollout restart deployment/frontend-deployment -n containertracking

# Rollback to previous version
kubectl rollout undo deployment/backend-deployment -n containertracking
kubectl rollout status deployment/backend-deployment -n containertracking
```

## 🔍 Troubleshooting

### Common Deployment Issues

#### Database Connection Issues
```bash
# Test database connectivity
pg_isready -h your-postgres-server -p 5432

# Check connection string format
# Correct: "Host=server;Database=db;Username=user;Password=pass"
# Incorrect: "Server=server;Database=db;..." (SQL Server format)
```

#### Container Startup Issues
```bash
# Check container logs
docker logs container-name

# Check resource usage
docker stats

# Inspect container configuration
docker inspect container-name
```

#### Kubernetes Issues
```bash
# Check pod events
kubectl describe pod pod-name -n containertracking

# Check resource usage
kubectl top pods -n containertracking

# Check service connectivity
kubectl port-forward service/backend-service 8080:80 -n containertracking
```

### Performance Optimization

#### Database Optimization
```sql
-- Add indexes for frequently queried columns
CREATE INDEX idx_container_number ON containers(container_number);
CREATE INDEX idx_ship_status ON ships(status);
CREATE INDEX idx_berth_assignment_dates ON berth_assignments(scheduled_arrival, scheduled_departure);

-- Analyze query performance
EXPLAIN ANALYZE SELECT * FROM containers WHERE status = 'Available';
```

#### Application Performance
```csharp
// Enable response caching
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// Configure compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

This comprehensive deployment guide should help you successfully deploy and maintain the Container Tracking & Port Operations System across various environments and platforms. Remember to follow security best practices and maintain regular backups of your production systems.