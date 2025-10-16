# 🏗️ Backend Architecture Guide

## System Architecture Overview

The Maersk Container Tracking backend follows **Clean Architecture** principles with **Domain-Driven Design (DDD)** patterns, ensuring maintainability, testability, and scalability.

## 🏛️ Architecture Layers

### 1. **Presentation Layer** (Controllers)
```
Controllers/
├── AuthController.cs           # Authentication & authorization
├── ContainersController.cs     # Container CRUD operations
├── ShipsController.cs          # Ship management
├── BerthsController.cs         # Berth operations
├── PortsController.cs          # Port management
├── BerthAssignmentsController.cs # Berth scheduling
├── AnalyticsController.cs      # Analytics & reporting
└── UserManagementController.cs # User administration
```

**Responsibilities:**
- HTTP request/response handling
- Input validation and model binding
- Authentication and authorization
- Response formatting and error handling

### 2. **Application Layer** (Services)
```
Services/
├── AuthService.cs              # Authentication logic
├── ContainerService.cs         # Container business logic
├── ShipService.cs             # Ship operations
├── BerthService.cs            # Berth management
├── AnalyticsService.cs        # Data analytics
├── UserManagementService.cs   # User operations
└── ComprehensiveDataSeedingService.cs # Data seeding
```

**Responsibilities:**
- Business logic implementation
- Data transformation and validation
- Service orchestration
- Domain rule enforcement

### 3. **Infrastructure Layer** (Repositories & Data)
```
Repositories/
├── IContainerRepository.cs     # Container data interface
├── ContainerRepository.cs      # Container data implementation
├── IShipRepository.cs          # Ship data interface
├── ShipRepository.cs          # Ship data implementation
├── IBerthRepository.cs        # Berth data interface
└── BerthRepository.cs         # Berth data implementation

Data/
└── ApplicationDbContext.cs     # Entity Framework context
```

**Responsibilities:**
- Data persistence and retrieval
- Database query optimization
- Entity relationship management
- Migration and schema management

### 4. **Domain Layer** (Models & Entities)
```
Models/
├── User.cs                    # User entity
├── Container.cs               # Container entity
├── Ship.cs                   # Ship entity
├── Port.cs                   # Port entity
├── Berth.cs                  # Berth entity
├── BerthAssignment.cs        # Berth assignment entity
├── ContainerMovement.cs      # Container movement tracking
└── Analytics.cs              # Analytics data model
```

## 🔧 Design Patterns & Principles

### 1. **Repository Pattern**
```csharp
public interface IContainerRepository
{
    Task<IEnumerable<Container>> GetAllAsync();
    Task<Container?> GetByIdAsync(int id);
    Task<Container> CreateAsync(Container container);
    Task<Container> UpdateAsync(Container container);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<Container>> SearchAsync(string query);
}
```

**Benefits:**
- Abstraction over data access
- Unit testing with mock repositories
- Separation of concerns
- Database technology independence

### 2. **Dependency Injection**
```csharp
// Program.cs
builder.Services.AddScoped<IContainerRepository, ContainerRepository>();
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
```

### 3. **Service Layer Pattern**
```csharp
public class ContainerService : IContainerService
{
    private readonly IContainerRepository _containerRepository;
    private readonly ILogger<ContainerService> _logger;

    public ContainerService(
        IContainerRepository containerRepository,
        ILogger<ContainerService> logger)
    {
        _containerRepository = containerRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<IEnumerable<ContainerDto>>> GetAllAsync()
    {
        // Business logic implementation
    }
}
```

## 🔐 Security Architecture

### 1. **JWT Authentication Flow**
```
1. User Login → AuthController.Login()
2. Validate Credentials → AuthService.ValidateUserAsync()
3. Generate JWT Token → JwtTokenGenerator.GenerateToken()
4. Return Token → Client stores token
5. API Requests → Include Bearer token in headers
6. Token Validation → JWT middleware validates on each request
```

### 2. **Role-Based Authorization**
```csharp
[Authorize(Roles = "Admin")]
public class UserManagementController : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.CreateUser)]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        // Implementation
    }
}
```

### 3. **Custom Authorization Attributes**
```csharp
public class HasPermissionAttribute : AuthorizeAttribute, IAuthorizationRequirement
{
    public string Permission { get; }

    public HasPermissionAttribute(string permission)
    {
        Permission = permission;
    }
}
```

## 💾 Data Architecture

### 1. **Entity Framework Core Configuration**
```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Container> Containers { get; set; }
    public DbSet<Ship> Ships { get; set; }
    public DbSet<Port> Ports { get; set; }
    public DbSet<Berth> Berths { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### 2. **Entity Relationships**
```
User ←→ BerthAssignment (One-to-Many)
Port ←→ Berth (One-to-Many)
Ship ←→ BerthAssignment (One-to-Many)
Berth ←→ BerthAssignment (One-to-Many)
Container ←→ ContainerMovement (One-to-Many)
Ship ←→ ShipContainer (Many-to-Many via junction table)
```

### 3. **Database Migrations**
```bash
# Create migration
dotnet ef migrations add AddNewFeature

# Update database
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigration
```

## 🔄 Application Flow

### 1. **Request Processing Pipeline**
```
HTTP Request
    ↓
Authentication Middleware
    ↓
Authorization Middleware
    ↓
Controller Action
    ↓
Service Layer
    ↓
Repository Layer
    ↓
Database
    ↓
Response Transformation
    ↓
HTTP Response
```

### 2. **Error Handling Flow**
```csharp
public class ExceptionMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

## 📊 Performance Considerations

### 1. **Database Optimizations**
- Proper indexing on frequently queried columns
- Lazy loading for navigation properties
- Query result caching for static data
- Connection pooling configuration

### 2. **API Performance**
```csharp
[ResponseCache(Duration = 300)] // 5-minute cache
public async Task<IActionResult> GetPorts()
{
    // Implementation with caching
}
```

### 3. **Async/Await Pattern**
```csharp
public async Task<ApiResponse<ContainerDto>> GetByIdAsync(int id)
{
    var container = await _containerRepository.GetByIdAsync(id);
    return ApiResponse<ContainerDto>.Success(container.ToDto());
}
```

## 🧪 Testing Architecture

### 1. **Unit Testing Structure**
```
Tests/
├── Controllers/
│   ├── ContainerControllerTests.cs
│   └── AuthControllerTests.cs
├── Services/
│   ├── ContainerServiceTests.cs
│   └── AuthServiceTests.cs
└── Repositories/
    └── ContainerRepositoryTests.cs
```

### 2. **Integration Testing**
```csharp
public class ContainerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    [Fact]
    public async Task GetContainers_ReturnsSuccessResponse()
    {
        // Test implementation
    }
}
```

## 🔧 Configuration Management

### 1. **Environment-Specific Settings**
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=ContainerTrackingDev"
  },
  "Jwt": {
    "SecretKey": "development-secret-key",
    "ExpiryMinutes": 1440
  }
}
```

### 2. **Dependency Registration**
```csharp
public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Service registrations
        return services;
    }
}
```

## 🚀 Deployment Architecture

### 1. **Production Configuration**
- Docker containerization support
- Azure App Service deployment
- PostgreSQL Flexible Server
- Application Insights logging
- Azure Key Vault for secrets

### 2. **Scalability Considerations**
- Stateless service design
- Database connection pooling
- Caching strategy implementation
- Load balancer support
- Horizontal scaling capability

## 📈 Monitoring & Logging

### 1. **Structured Logging**
```csharp
_logger.LogInformation("Container {ContainerId} updated successfully by user {UserId}", 
    containerId, userId);
```

### 2. **Health Checks**
```csharp
builder.Services.AddHealthChecks()
    .AddDbContext<ApplicationDbContext>()
    .AddCheck<DatabaseHealthCheck>("database");
```

## 🔗 External Integrations

### 1. **Kafka Integration** (Optional)
- Event streaming for real-time updates
- Container movement notifications
- Ship status broadcasting

### 2. **API Versioning**
```csharp
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/containers")]
public class ContainersController : ControllerBase
```

This architecture ensures maintainability, scalability, and follows industry best practices for enterprise-grade applications.