# 🧪 Testing Guide

## Testing Strategy Overview

This guide covers comprehensive testing strategies for the Maersk Container Tracking backend API, including unit testing, integration testing, authentication testing, and manual testing approaches.

## 🎯 Testing Pyramid

```
    🔺 E2E Tests (Postman Collections)
   🔺🔺 Integration Tests (.NET)  
  🔺🔺🔺 Unit Tests (.NET)
 🔺🔺🔺🔺 Manual API Tests
```

## 🚀 Quick Start Testing

### 1. **Authentication Test (Essential First Step)**

#### Using PowerShell Script:
```powershell
# Navigate to scripts folder
cd scripts

# Run authentication test script
.\test-auth.ps1

# Expected output:
# ✅ Authentication successful
# ✅ Berths: Found 20 records
# ✅ Containers: Found 300+ records
```

#### Using cURL:
```bash
# Login to get JWT token
curl -X POST "http://localhost:5221/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'

# Use returned token for API calls
export TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Test protected endpoint
curl -X GET "http://localhost:5221/api/containers" \
  -H "Authorization: Bearer $TOKEN"
```

### 2. **Postman Collections (Recommended)**

#### Import Collections:
1. **Authentication Collection**: `docs/backend/Container-Tracking-API-Auth.postman_collection.json`
2. **Main API Collection**: `docs/backend/Container-Tracking-API.postman_collection.json`

#### Test Workflow:
```
1. Run "Login (Admin)" → Auto-saves JWT token
2. Run "Get All Containers" → Uses saved token
3. Run "Create Container" → Test POST operations
4. Run "Update Container" → Test PUT operations
5. Run "Delete Container" → Test DELETE operations
```

## 🔐 Authentication Testing

### Default Test Credentials
| Role | Username | Password | Access Level |
|------|----------|-----------|-------------|
| Admin | `admin` | `Admin123!` | Full system access |
| Port Manager | `portmanager` | `Port123!` | Port operations |
| Operator | `operator` | `Op123!` | Daily operations |
| Viewer | `viewer` | `View123!` | Read-only access |

### Authentication Test Scenarios

#### 1. **Valid Login Test**
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}

Expected Response (200 OK):
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "user": {
      "id": "1",
      "username": "admin",
      "role": "Admin"
    }
  }
}
```

#### 2. **Invalid Credentials Test**
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "wrongpassword"
}

Expected Response (401 Unauthorized):
{
  "success": false,
  "message": "Invalid credentials"
}
```

#### 3. **Token Validation Test**
```http
GET /api/containers
Authorization: Bearer {valid_token}

Expected Response (200 OK):
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 300
  }
}
```

#### 4. **Expired Token Test**
```http
GET /api/containers
Authorization: Bearer {expired_token}

Expected Response (401 Unauthorized):
{
  "success": false,
  "message": "Token has expired"
}
```

## 📊 API Endpoint Testing

### 1. **Container Management Tests**

#### Get All Containers:
```http
GET /api/containers
Authorization: Bearer {token}

# Test with pagination
GET /api/containers?page=1&size=10

# Test with filtering
GET /api/containers?status=Available&type=Standard
```

#### Create Container:
```http
POST /api/containers
Authorization: Bearer {token}
Content-Type: application/json

{
  "containerNumber": "TEST123456",
  "type": "Standard",
  "size": "20ft",
  "weight": 15000.00,
  "cargo": "Test Cargo",
  "status": "Available",
  "location": "Test Port"
}

Expected: 201 Created
```

#### Update Container:
```http
PUT /api/containers/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "status": "In Transit",
  "location": "Updated Location"
}

Expected: 200 OK
```

#### Delete Container:
```http
DELETE /api/containers/1
Authorization: Bearer {token}

Expected: 204 No Content
```

### 2. **Ship Management Tests**

#### Create Ship Test:
```http
POST /api/ships
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Test Ship",
  "imoNumber": "IMO9999999",
  "flag": "Denmark",
  "type": "Container Ship",
  "capacity": 20000,
  "status": "At Port"
}
```

### 3. **Berth Operations Tests**

#### Get Available Berths:
```http
GET /api/berths?status=Available
Authorization: Bearer {token}
```

#### Create Berth Assignment:
```http
POST /api/berth-assignments
Authorization: Bearer {token}
Content-Type: application/json

{
  "shipId": 1,
  "berthId": 1,
  "scheduledArrival": "2025-10-17T08:00:00Z",
  "scheduledDeparture": "2025-10-18T16:00:00Z"
}
```

## 🧪 Unit Testing (.NET)

### Setting Up Unit Tests

#### 1. **Create Test Project**
```bash
# Create test project
dotnet new xunit -n Tests.UnitTests
cd Tests.UnitTests

# Add references
dotnet add reference ../../backend/backend.csproj
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Moq
dotnet add package FluentAssertions
```

#### 2. **Sample Unit Test**
```csharp
public class ContainerServiceTests
{
    private readonly Mock<IContainerRepository> _mockRepository;
    private readonly ContainerService _service;

    public ContainerServiceTests()
    {
        _mockRepository = new Mock<IContainerRepository>();
        _service = new ContainerService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ReturnsContainer()
    {
        // Arrange
        var containerId = 1;
        var expectedContainer = new Container 
        { 
            Id = containerId, 
            ContainerNumber = "TEST123" 
        };
        
        _mockRepository
            .Setup(r => r.GetByIdAsync(containerId))
            .ReturnsAsync(expectedContainer);

        // Act
        var result = await _service.GetByIdAsync(containerId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Id.Should().Be(containerId);
        result.Data.ContainerNumber.Should().Be("TEST123");
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var containerId = 999;
        
        _mockRepository
            .Setup(r => r.GetByIdAsync(containerId))
            .ReturnsAsync((Container)null);

        // Act
        var result = await _service.GetByIdAsync(containerId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }
}
```

#### 3. **Run Unit Tests**
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "ContainerServiceTests"

# Run tests with verbose output
dotnet test --logger "console;verbosity=detailed"
```

## 🔗 Integration Testing

### 1. **Integration Test Setup**
```csharp
public class ContainerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ContainerControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetContainers_WithValidToken_ReturnsSuccess()
    {
        // Arrange
        var token = await GetValidTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/containers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<dynamic>>(content);
        result.Success.Should().BeTrue();
    }

    private async Task<string> GetValidTokenAsync()
    {
        var loginRequest = new
        {
            username = "admin",
            password = "Admin123!"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>();
        
        return loginResult.Data.Token;
    }
}
```

### 2. **Database Integration Tests**
```csharp
public class ContainerRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ContainerRepository _repository;

    public ContainerRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ContainerRepository(_context);
        
        SeedTestData();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllContainers()
    {
        // Act
        var containers = await _repository.GetAllAsync();

        // Assert
        containers.Should().HaveCount(3);
    }

    private void SeedTestData()
    {
        _context.Containers.AddRange(
            new Container { ContainerNumber = "TEST001", Type = "Standard" },
            new Container { ContainerNumber = "TEST002", Type = "Refrigerated" },
            new Container { ContainerNumber = "TEST003", Type = "Tank" }
        );
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## 🚀 Performance Testing

### 1. **Load Testing with Artillery**
```yaml
# artillery-config.yml
config:
  target: 'http://localhost:5221'
  phases:
    - duration: 60
      arrivalRate: 5
scenarios:
  - name: "Container API Load Test"
    flow:
      - post:
          url: "/api/auth/login"
          json:
            username: "admin"
            password: "Admin123!"
          capture:
            - json: "$.data.token"
              as: "token"
      - get:
          url: "/api/containers"
          headers:
            Authorization: "Bearer {{ token }}"
```

```bash
# Run load test
npm install -g artillery
artillery run artillery-config.yml
```

### 2. **Benchmark Testing**
```bash
# Using Apache Bench
ab -n 1000 -c 10 -H "Authorization: Bearer TOKEN" http://localhost:5221/api/containers

# Using curl for response time
curl -w "@curl-format.txt" -o /dev/null -s "http://localhost:5221/api/health/live"
```

## 🐛 Error Testing

### 1. **Validation Error Tests**
```http
POST /api/containers
Authorization: Bearer {token}
Content-Type: application/json

{
  "containerNumber": "",  // Invalid: empty
  "type": "InvalidType",  // Invalid: not in enum
  "weight": -100          // Invalid: negative
}

Expected Response (400 Bad Request):
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "containerNumber": ["Container number is required"],
    "type": ["Invalid container type"],
    "weight": ["Weight must be positive"]
  }
}
```

### 2. **Authorization Error Tests**
```http
GET /api/users  # Admin only endpoint
Authorization: Bearer {operator_token}

Expected Response (403 Forbidden):
{
  "success": false,
  "message": "Insufficient permissions"
}
```

### 3. **Not Found Error Tests**
```http
GET /api/containers/99999
Authorization: Bearer {token}

Expected Response (404 Not Found):
{
  "success": false,
  "message": "Container not found"
}
```

## 📊 Test Data Management

### 1. **Database Seeding for Tests**
```csharp
public static class TestDataSeeder
{
    public static void SeedTestData(ApplicationDbContext context)
    {
        // Clear existing data
        context.Containers.RemoveRange(context.Containers);
        context.Ships.RemoveRange(context.Ships);
        
        // Seed test data
        var containers = new List<Container>
        {
            new() { ContainerNumber = "TEST001", Type = "Standard", Status = "Available" },
            new() { ContainerNumber = "TEST002", Type = "Refrigerated", Status = "In Transit" }
        };
        
        context.Containers.AddRange(containers);
        context.SaveChanges();
    }
}
```

### 2. **Test Environment Configuration**
```json
// appsettings.Testing.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=ContainerTrackingTest"
  },
  "Jwt": {
    "SecretKey": "test-secret-key-for-testing-only-min-32-chars",
    "ExpiryMinutes": 60
  }
}
```

## 🔍 Testing Checklist

### Pre-Testing Setup ✅
- [ ] Backend server running (`dotnet run`)
- [ ] Database accessible and seeded
- [ ] Postman collections imported
- [ ] Environment variables configured
- [ ] Test credentials verified

### Authentication Tests ✅
- [ ] Valid login returns JWT token
- [ ] Invalid credentials return 401
- [ ] Expired token returns 401
- [ ] Protected endpoints require authentication

### CRUD Operations Tests ✅
- [ ] GET endpoints return data with valid token
- [ ] POST endpoints create resources
- [ ] PUT endpoints update resources
- [ ] DELETE endpoints remove resources
- [ ] Validation errors return 400

### Authorization Tests ✅
- [ ] Admin can access all endpoints
- [ ] Port Manager can access port operations
- [ ] Operator can access daily operations
- [ ] Viewer has read-only access

### Error Handling Tests ✅
- [ ] 404 for non-existent resources
- [ ] 400 for invalid data
- [ ] 403 for insufficient permissions
- [ ] 500 errors are handled gracefully

### Performance Tests ✅
- [ ] Response times under 200ms for simple queries
- [ ] Pagination works for large datasets
- [ ] Database queries are optimized
- [ ] No memory leaks in long-running tests

## 🆘 Troubleshooting Test Issues

### Common Test Problems:

1. **Database Connection Failures**
```bash
# Check PostgreSQL status
docker ps | grep postgres
# Restart if needed
docker restart postgres-container
```

2. **Authentication Token Issues**
```bash
# Verify token expiry
# Re-run login request to get fresh token
```

3. **Port Conflicts**
```bash
# Check if backend is running
curl http://localhost:5221/api/health/live
```

4. **Test Data Issues**
```bash
# Reset database
dotnet ef database drop --force
dotnet ef database update
```

## 📈 Continuous Integration Testing

### GitHub Actions Workflow:
```yaml
name: Backend Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      postgres:
        image: postgres:14
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: ContainerTrackingTest
        ports:
          - 5432:5432
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 8.0.x
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Run tests
      run: dotnet test --logger trx --results-directory "TestResults"
      
    - name: Publish test results
      uses: dorny/test-reporter@v1
      if: success() || failure()
      with:
        name: .NET Tests
        path: TestResults/*.trx
        reporter: dotnet-trx
```

This comprehensive testing guide ensures your backend API is thoroughly tested, reliable, and ready for production deployment.