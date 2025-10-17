# 🔧 Troubleshooting Guide

## Overview

This comprehensive troubleshooting guide helps you diagnose and resolve common issues in the Maersk Container Tracking & Port Operations System. Whether you're dealing with deployment problems, runtime errors, or performance issues, this guide provides step-by-step solutions and preventive measures.

## 🚨 Emergency Response Protocol

### Severity Levels
- **P0 - Critical**: System down, data loss, security breach
- **P1 - High**: Major functionality broken, significant user impact
- **P2 - Medium**: Feature issues, minor user impact
- **P3 - Low**: Cosmetic issues, minimal impact

### Emergency Contacts
- **On-Call Engineer**: +1-800-EMERGENCY
- **DevOps Team**: devops@containertracking.com
- **Security Team**: security@containertracking.com
- **Database Admin**: dba@containertracking.com

### Incident Response Steps
1. **Assess** severity and impact
2. **Notify** appropriate teams
3. **Document** issue details
4. **Implement** immediate fix or rollback
5. **Conduct** post-incident review

## 🔍 Diagnostic Tools & Commands

### System Health Checks

#### Quick Health Assessment
```bash
# Check application status
curl -f http://localhost:5266/api/health/live
curl -f http://localhost:5266/api/health/ready

# Check frontend accessibility
curl -f http://localhost:5173

# Database connectivity test
pg_isready -h localhost -p 5432

# Redis connectivity test
redis-cli ping
```

#### Detailed System Information
```bash
# Check system resources
free -h                    # Memory usage
df -h                      # Disk usage
top                        # CPU usage and processes
netstat -tulnp            # Network connections

# Check Docker containers
docker ps -a              # All containers
docker stats              # Resource usage
docker system df          # Docker disk usage

# Check Kubernetes (if applicable)
kubectl get pods -A       # All pods
kubectl top nodes         # Node resource usage
kubectl get events --sort-by=.metadata.creationTimestamp
```

### Log Analysis

#### Application Logs Location
```bash
# Backend logs (.NET)
/app/logs/                           # Docker container
/var/log/containertracking/         # Linux host
~/AppData/Local/ContainerTracking/  # Windows host

# Frontend logs (Nginx)
/var/log/nginx/access.log
/var/log/nginx/error.log

# System logs
/var/log/syslog               # Linux system log
/var/log/dmesg               # Kernel messages
```

#### Log Analysis Commands
```bash
# Real-time log monitoring
tail -f /app/logs/app.log

# Search for errors
grep -i error /app/logs/app.log
grep -i exception /app/logs/app.log

# Filter by date range
sed -n '/2024-01-15/,/2024-01-16/p' /app/logs/app.log

# Count error occurrences
grep -c "ERROR" /app/logs/app.log

# Extract specific log levels
awk '/INFO|WARN|ERROR/' /app/logs/app.log
```

## 🚀 Common Issues & Solutions

### 1. Application Won't Start

#### Backend Startup Issues

**Symptom**: .NET application fails to start
```
Application startup exception: System.ArgumentNullException: Value cannot be null. (Parameter 'connectionString')
```

**Diagnosis**:
```bash
# Check environment variables
printenv | grep -i connection
echo $ConnectionStrings__DefaultConnection

# Verify appsettings.json
cat backend/appsettings.json | jq '.ConnectionStrings'

# Test configuration loading
dotnet run --environment Development --verbosity diagnostic
```

**Solutions**:
1. **Fix connection string**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=container_tracking;Username=postgres;Password=your_password"
  }
}
```

2. **Set environment variables**:
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Database=container_tracking;Username=postgres;Password=your_password"
```

3. **Docker environment**:
```yaml
# docker-compose.yml
environment:
  - ConnectionStrings__DefaultConnection=Host=postgres;Database=container_tracking;Username=postgres;Password=dev_password
```

#### Frontend Startup Issues

**Symptom**: Vue.js application shows blank page
```
Failed to load resource: net::ERR_CONNECTION_REFUSED
```

**Diagnosis**:
```bash
# Check if Vite dev server is running
lsof -i :5173
netstat -tulnp | grep 5173

# Check browser console for errors
# Open Developer Tools → Console

# Verify build output
npm run build
ls -la frontend/dist/
```

**Solutions**:
1. **Start development server**:
```bash
cd frontend
npm run dev
```

2. **Fix API URL configuration**:
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5266/api
```

3. **Clear cache and reinstall**:
```bash
cd frontend
rm -rf node_modules package-lock.json
npm cache clean --force
npm install
```

### 2. Database Connection Problems

#### PostgreSQL Connection Issues

**Symptom**: Cannot connect to database
```
Npgsql.NpgsqlException: Connection refused
```

**Diagnosis**:
```bash
# Check PostgreSQL status
sudo systemctl status postgresql

# Test direct connection
psql -h localhost -U postgres -d container_tracking

# Check PostgreSQL logs
sudo tail -f /var/log/postgresql/postgresql-*.log

# Verify port is listening
sudo netstat -tlnp | grep 5432
```

**Solutions**:
1. **Start PostgreSQL service**:
```bash
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

2. **Fix PostgreSQL configuration**:
```bash
# Edit postgresql.conf
sudo nano /etc/postgresql/14/main/postgresql.conf

# Ensure these settings:
listen_addresses = 'localhost'
port = 5432

# Edit pg_hba.conf for authentication
sudo nano /etc/postgresql/14/main/pg_hba.conf

# Add line for local connections:
host    all             all             127.0.0.1/32            md5
```

3. **Create database and user**:
```sql
-- Connect as postgres user
sudo -u postgres psql

-- Create database
CREATE DATABASE container_tracking;

-- Create user
CREATE USER app_user WITH PASSWORD 'secure_password';

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE container_tracking TO app_user;
```

#### Entity Framework Migration Issues

**Symptom**: Database schema mismatch
```
InvalidOperationException: The model backing the 'ApplicationDbContext' context has changed since the database was created
```

**Diagnosis**:
```bash
# Check migration status
cd backend
dotnet ef migrations list

# Check database schema
dotnet ef database update --dry-run

# Compare models
dotnet ef migrations has-pending-model-changes
```

**Solutions**:
1. **Apply pending migrations**:
```bash
dotnet ef database update
```

2. **Reset database** (development only):
```bash
dotnet ef database drop
dotnet ef database update
```

3. **Create new migration**:
```bash
dotnet ef migrations add FixSchemaIssue
dotnet ef database update
```

### 3. Authentication & Authorization Problems

#### JWT Token Issues

**Symptom**: Authentication fails with valid credentials
```
401 Unauthorized: Invalid token
```

**Diagnosis**:
```bash
# Check JWT configuration
grep -r "JwtSettings" backend/appsettings.json

# Decode JWT token (using online tool or jwt-cli)
echo "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9..." | base64 -d

# Check token expiration
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5266/api/auth/profile
```

**Solutions**:
1. **Fix JWT configuration**:
```json
{
  "JwtSettings": {
    "Key": "your-256-bit-secret-key-here-must-be-at-least-32-characters",
    "Issuer": "ContainerTracking",
    "Audience": "ContainerTrackingUsers",
    "DurationInMinutes": 60
  }
}
```

2. **Regenerate JWT secret**:
```bash
# Generate new 256-bit key
openssl rand -base64 32
```

3. **Clear stored tokens** (frontend):
```javascript
// Clear localStorage
localStorage.removeItem('authToken');
localStorage.removeItem('refreshToken');

// Clear sessionStorage
sessionStorage.clear();
```

#### CORS Issues

**Symptom**: Browser blocks API requests
```
Access to XMLHttpRequest blocked by CORS policy
```

**Diagnosis**:
```bash
# Check CORS configuration
grep -r "AddCors" backend/Program.cs

# Test with curl (bypasses CORS)
curl -X GET http://localhost:5266/api/containers -H "Origin: http://localhost:5173"
```

**Solutions**:
1. **Configure CORS properly**:
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("DevelopmentPolicy");
```

2. **Environment-specific CORS**:
```json
{
  "AllowedOrigins": [
    "http://localhost:5173",
    "https://containertracking.com",
    "https://www.containertracking.com"
  ]
}
```

### 4. Performance Issues

#### Slow API Responses

**Symptom**: API requests taking >5 seconds
```
Request timeout after 10000ms
```

**Diagnosis**:
```bash
# Check API response times
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5266/api/containers

# curl-format.txt content:
# time_total: %{time_total}s
# time_connect: %{time_connect}s
# time_starttransfer: %{time_starttransfer}s

# Check database query performance
# Enable SQL logging in appsettings.json
```

**Solutions**:
1. **Database optimization**:
```sql
-- Add indexes for frequently queried columns
CREATE INDEX idx_containers_status ON containers(status);
CREATE INDEX idx_containers_location ON containers(current_location);
CREATE INDEX idx_berth_assignments_dates ON berth_assignments(scheduled_arrival, scheduled_departure);

-- Analyze query performance
EXPLAIN ANALYZE SELECT * FROM containers WHERE status = 'Available';
```

2. **Enable response caching**:
```csharp
// Program.cs
builder.Services.AddResponseCaching();
builder.Services.AddMemoryCache();

app.UseResponseCaching();

// Controller
[ResponseCache(Duration = 300)] // 5 minutes
public async Task<IActionResult> GetContainers()
{
    // Method implementation
}
```

3. **Implement pagination**:
```csharp
public async Task<PagedResult<Container>> GetContainersAsync(int page, int pageSize)
{
    var totalCount = await _context.Containers.CountAsync();
    var containers = await _context.Containers
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    return new PagedResult<Container>
    {
        Items = containers,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}
```

#### Memory Issues

**Symptom**: High memory usage or out of memory errors
```
System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown
```

**Diagnosis**:
```bash
# Check memory usage
free -m
ps aux --sort=-%mem | head

# Check Docker container memory
docker stats

# Check for memory leaks in .NET
dotnet-counters monitor --process-id $(pgrep dotnet)
```

**Solutions**:
1. **Optimize Entity Framework queries**:
```csharp
// Use AsNoTracking for read-only queries
var containers = await _context.Containers
    .AsNoTracking()
    .Where(c => c.Status == "Available")
    .ToListAsync();

// Dispose contexts properly
using var context = new ApplicationDbContext(options);
```

2. **Configure garbage collection**:
```json
{
  "System.GC.Server": true,
  "System.GC.Concurrent": true,
  "System.GC.RetainVM": true
}
```

3. **Set Docker memory limits**:
```yaml
# docker-compose.yml
services:
  backend:
    image: containertracking/backend
    deploy:
      resources:
        limits:
          memory: 1G
        reservations:
          memory: 512M
```

### 5. Docker & Container Issues

#### Container Won't Start

**Symptom**: Docker container exits immediately
```
container exited with code 1
```

**Diagnosis**:
```bash
# Check container logs
docker logs container-name

# Check container configuration
docker inspect container-name

# Run container interactively
docker run -it --rm containertracking/backend /bin/bash
```

**Solutions**:
1. **Fix Dockerfile**:
```dockerfile
# Ensure proper base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set working directory
WORKDIR /app

# Copy and set permissions
COPY --from=build /app/publish .
RUN chmod +x /app/backend

# Use proper entrypoint
ENTRYPOINT ["dotnet", "backend.dll"]
```

2. **Check environment variables**:
```bash
# List all environment variables
docker run --rm containertracking/backend env

# Test with specific variables
docker run -e ASPNETCORE_ENVIRONMENT=Development containertracking/backend
```

3. **Volume mount issues**:
```yaml
# Fix volume permissions
volumes:
  - ./data:/app/data:rw
  - ./logs:/app/logs:rw

# Set proper ownership
- chown -R 1000:1000 ./data ./logs
```

#### Docker Compose Issues

**Symptom**: Services can't communicate
```
Name or service not known: postgres
```

**Diagnosis**:
```bash
# Check network configuration
docker network ls
docker network inspect container-tracking_default

# Check service names in compose file
docker-compose config

# Test service connectivity
docker-compose exec backend ping postgres
```

**Solutions**:
1. **Fix service names**:
```yaml
# docker-compose.yml
services:
  postgres:  # This becomes the hostname
    image: postgres:14
  
  backend:
    image: containertracking/backend
    depends_on:
      - postgres
    environment:
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=container_tracking;...
```

2. **Network configuration**:
```yaml
networks:
  app-network:
    driver: bridge

services:
  postgres:
    networks:
      - app-network
  backend:
    networks:
      - app-network
```

### 6. Frontend Issues

#### API Integration Problems

**Symptom**: Frontend can't reach backend API
```
TypeError: Failed to fetch
```

**Diagnosis**:
```bash
# Check API URL configuration
grep -r "VITE_API_BASE_URL" frontend/

# Test API directly
curl http://localhost:5266/api/health/live

# Check browser network tab
# Open Developer Tools → Network
```

**Solutions**:
1. **Fix API base URL**:
```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5266/api

# .env.production  
VITE_API_BASE_URL=https://api.containertracking.com/api
```

2. **Configure axios properly**:
```typescript
// src/services/api.ts
import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Add request interceptor for auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})
```

3. **Handle errors properly**:
```typescript
// src/composables/useApi.ts
export function useApi() {
  const handleError = (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Redirect to login
      router.push('/login')
    } else if (error.response?.status >= 500) {
      // Show server error message
      showNotification('Server error occurred', 'error')
    }
  }
  
  return { handleError }
}
```

#### Build Issues

**Symptom**: Frontend build fails
```
Module not found: Error: Can't resolve './missing-component'
```

**Diagnosis**:
```bash
# Check for missing files
find frontend/src -name "*.vue" -o -name "*.ts" | grep -i missing

# Check import statements
grep -r "missing-component" frontend/src/

# Clear cache and reinstall
rm -rf frontend/node_modules frontend/.vite
npm cache clean --force
```

**Solutions**:
1. **Fix import paths**:
```typescript
// Use relative imports
import Component from './Component.vue'
import { useStore } from '../stores/store'

// Use absolute imports with path mapping
import Component from '@/components/Component.vue'
```

2. **Check tsconfig.json**:
```json
{
  "compilerOptions": {
    "baseUrl": ".",
    "paths": {
      "@/*": ["src/*"]
    }
  }
}
```

3. **Verify dependencies**:
```bash
npm audit
npm update
npm install --legacy-peer-deps  # If peer dependency conflicts
```

## 🔧 Advanced Debugging Techniques

### .NET Application Debugging

#### Enable Detailed Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "Microsoft.EntityFrameworkCore.Database.Connection": "Information"
    }
  }
}
```

#### Performance Profiling
```bash
# Install dotnet-trace
dotnet tool install --global dotnet-trace

# Collect performance trace
dotnet-trace collect --process-id $(pgrep dotnet) --duration 00:00:30

# Analyze with PerfView or Visual Studio
```

#### Memory Dump Analysis
```bash
# Create memory dump
dotnet-dump collect --process-id $(pgrep dotnet)

# Analyze with dotnet-dump
dotnet-dump analyze core_dump_file
```

### Database Debugging

#### Query Analysis
```sql
-- Enable query logging in PostgreSQL
ALTER SYSTEM SET log_statement = 'all';
ALTER SYSTEM SET log_min_duration_statement = 100;  -- Log queries > 100ms
SELECT pg_reload_conf();

-- Check slow queries
SELECT query, calls, total_time, mean_time 
FROM pg_stat_statements 
ORDER BY mean_time DESC 
LIMIT 10;
```

#### Connection Pool Monitoring
```sql
-- Check active connections
SELECT datname, numbackends 
FROM pg_stat_database 
WHERE datname = 'container_tracking';

-- Check connection pool status (.NET)
-- Add to appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=container_tracking;Username=user;Password=pass;Minimum Pool Size=5;Maximum Pool Size=100;Connection Lifetime=300"
}
```

### Network Debugging

#### API Request Tracing
```bash
# Use tcpdump to capture HTTP traffic
sudo tcpdump -i any -A 'port 5266'

# Use Wireshark for detailed analysis
wireshark &

# Check firewall rules
sudo iptables -L
sudo ufw status
```

#### DNS Resolution
```bash
# Test DNS resolution
nslookup api.containertracking.com
dig api.containertracking.com

# Check /etc/hosts file
cat /etc/hosts

# Test connectivity
telnet api.containertracking.com 443
nc -zv api.containertracking.com 443
```

## 📊 Monitoring & Alerting Setup

### Application Performance Monitoring

#### Custom Health Checks
```csharp
// Custom health check
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    
    public DatabaseHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            var containerCount = await _context.Containers.CountAsync(cancellationToken);
            
            return HealthCheckResult.Healthy($"Database accessible. Container count: {containerCount}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}

// Register in Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
```

#### Metrics Collection
```csharp
// Custom metrics with System.Diagnostics.Metrics
public class ApplicationMetrics
{
    private readonly Meter _meter;
    private readonly Counter<int> _containerCreatedCounter;
    private readonly Histogram<double> _apiResponseTime;
    
    public ApplicationMetrics()
    {
        _meter = new Meter("ContainerTracking");
        _containerCreatedCounter = _meter.CreateCounter<int>("containers_created_total");
        _apiResponseTime = _meter.CreateHistogram<double>("api_response_time_seconds");
    }
    
    public void RecordContainerCreated() => _containerCreatedCounter.Add(1);
    public void RecordApiResponseTime(double seconds) => _apiResponseTime.Record(seconds);
}
```

### Log Aggregation

#### Structured Logging
```csharp
// Use structured logging with Serilog
Log.Information("Container {ContainerNumber} created by user {UserId} at port {PortName}", 
    container.ContainerNumber, userId, portName);

Log.Warning("High berth utilization {Utilization}% at port {PortId}", 
    utilizationPercent, portId);

Log.Error(exception, "Failed to process berth assignment {AssignmentId} for ship {ShipId}", 
    assignmentId, shipId);
```

#### Log Shipping to ELK Stack
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://elasticsearch:9200",
          "indexFormat": "containertracking-logs-{0:yyyy.MM.dd}"
        }
      }
    ]
  }
}
```

### Alerting Rules

#### Prometheus Alert Rules
```yaml
# alerts.yml
groups:
- name: containertracking.alerts
  rules:
  - alert: ApplicationDown
    expr: up{job="containertracking-backend"} == 0
    for: 1m
    labels:
      severity: critical
    annotations:
      summary: "Container Tracking application is down"
      description: "The Container Tracking backend has been down for more than 1 minute"
      
  - alert: HighErrorRate
    expr: rate(http_requests_total{status=~"5.."}[5m]) > 0.1
    for: 2m
    labels:
      severity: warning
    annotations:
      summary: "High error rate detected"
      description: "Error rate is {{ $value }} errors per second"
      
  - alert: DatabaseConnectionFailure
    expr: database_connections_failed_total > 0
    for: 30s
    labels:
      severity: critical
    annotations:
      summary: "Database connection failures"
      description: "{{ $value }} database connection failures in the last 30 seconds"
```

## 🚀 Performance Optimization

### Database Optimization

#### Query Optimization
```sql
-- Identify slow queries
SELECT 
    query,
    calls,
    total_time,
    mean_time,
    (100 * total_time / sum(total_time) OVER()) AS percentage_cpu
FROM pg_stat_statements
ORDER BY total_time DESC
LIMIT 20;

-- Add strategic indexes
CREATE INDEX CONCURRENTLY idx_containers_status_location 
ON containers(status, current_location) 
WHERE status IN ('Available', 'In Transit');

-- Update table statistics
ANALYZE containers;
ANALYZE ships;
ANALYZE berth_assignments;
```

#### Connection Pool Tuning
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=container_tracking;Username=user;Password=pass;Pooling=true;MinPoolSize=10;MaxPoolSize=100;ConnectionLifeTime=300;CommandTimeout=30"
  }
}
```

### Application Optimization

#### Caching Strategy
```csharp
// Distributed caching
public class ContainerService
{
    private readonly IDistributedCache _cache;
    private readonly IContainerRepository _repository;
    
    public async Task<Container> GetContainerAsync(int id)
    {
        string cacheKey = $"container:{id}";
        var cachedContainer = await _cache.GetStringAsync(cacheKey);
        
        if (cachedContainer != null)
        {
            return JsonSerializer.Deserialize<Container>(cachedContainer);
        }
        
        var container = await _repository.GetByIdAsync(id);
        
        if (container != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(container), 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }
        
        return container;
    }
}
```

#### Asynchronous Processing
```csharp
// Background job processing
public class ContainerMovementProcessor : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ContainerMovementProcessor> _logger;
    private Timer _timer;
    
    public async Task ProcessPendingMovements()
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IContainerMovementRepository>();
        
        var pendingMovements = await repository.GetPendingMovementsAsync();
        
        var tasks = pendingMovements.Select(ProcessMovementAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessMovementAsync(ContainerMovement movement)
    {
        // Process individual movement
        await Task.Delay(100); // Simulate processing time
    }
}
```

This comprehensive troubleshooting guide should help you quickly identify and resolve most issues you'll encounter with the Container Tracking & Port Operations System. Remember to always document solutions and share knowledge with your team to prevent recurring issues.