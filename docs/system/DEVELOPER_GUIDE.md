# 👨‍💻 Developer Guide

## Welcome to the Development Team

This comprehensive guide will help you set up, understand, and contribute to the Maersk Container Tracking & Port Operations System. Whether you're a new team member or an experienced developer joining the project, this guide provides everything you need to become productive quickly.

## 📋 Prerequisites

### Required Software
- **Node.js**: 20.x LTS or higher
- **.NET SDK**: 8.0 or higher
- **PostgreSQL**: 14.x or higher
- **Git**: Latest version
- **Docker**: Latest version (optional but recommended)
- **Visual Studio Code**: Recommended IDE

### Recommended Extensions (VS Code)
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "vue.volar",
    "vue.vscode-typescript-vue-plugin",
    "bradlc.vscode-tailwindcss",
    "ms-vscode.vscode-typescript-next",
    "ms-vscode-remote.remote-containers",
    "ms-azuretools.vscode-docker",
    "humao.rest-client"
  ]
}
```

## 🚀 Quick Start Guide

### 1. Repository Setup
```bash
# Clone the repository
git clone https://github.com/yourusername/container-tracking-maersk.git
cd container-tracking-maersk

# Install backend dependencies
cd backend
dotnet restore

# Install frontend dependencies  
cd ../frontend
npm install

# Return to root
cd ..
```

### 2. Environment Configuration

#### Backend Configuration
Create `backend/appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=container_tracking_dev;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Key": "your-super-secret-jwt-key-for-development-minimum-256-bits",
    "Issuer": "ContainerTracking",
    "Audience": "ContainerTrackingUsers",
    "DurationInMinutes": 60
  },
  "AllowedOrigins": ["http://localhost:5173", "http://127.0.0.1:5173"]
}
```

#### Frontend Configuration
Create `frontend/.env.development`:
```bash
VITE_API_BASE_URL=http://localhost:5266/api
VITE_APP_TITLE=Container Tracking - Development
VITE_ENABLE_DEBUG=true
```

### 3. Database Setup
```bash
# Start PostgreSQL (if using Docker)
docker run --name postgres-dev -e POSTGRES_PASSWORD=your_password -d -p 5432:5432 postgres:14

# Create database
createdb -h localhost -U postgres container_tracking_dev

# Run migrations
cd backend
dotnet ef database update
```

### 4. Run the Applications
```bash
# Terminal 1: Backend
cd backend
dotnet run

# Terminal 2: Frontend (new terminal)
cd frontend
npm run dev
```

### 5. Access the Application
- **Frontend**: http://localhost:5173
- **Backend API**: http://localhost:5266
- **API Documentation**: http://localhost:5266/swagger

## 🏗️ Project Structure Deep Dive

### Backend Architecture (.NET 8)
```
backend/
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration files
├── backend.csproj            # Project dependencies
│
├── Controllers/              # API Controllers
│   ├── AuthController.cs     # Authentication endpoints
│   ├── ContainersController.cs
│   ├── ShipsController.cs
│   ├── BerthsController.cs
│   └── AnalyticsController.cs
│
├── Models/                   # Domain entities
│   ├── Container.cs
│   ├── Ship.cs
│   ├── Berth.cs
│   └── User.cs
│
├── DTOs/                     # Data Transfer Objects
│   ├── ApiResponse.cs        # Standard API response
│   ├── ContainerDto.cs
│   └── AuthDto.cs
│
├── Data/                     # Database context
│   └── ApplicationDbContext.cs
│
├── Services/                 # Business logic
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   └── ContainerService.cs
│
├── Repositories/             # Data access layer
│   ├── IGenericRepository.cs
│   ├── GenericRepository.cs
│   └── ContainerRepository.cs
│
├── Middleware/               # Custom middleware
│   ├── ExceptionMiddleware.cs
│   └── SecurityHeadersMiddleware.cs
│
└── Extensions/               # Extension methods
    ├── ServiceRegistrationExtensions.cs
    └── MiddlewareExtensions.cs
```

### Frontend Architecture (Vue.js 3)
```
frontend/src/
├── main.ts                   # Application entry point
├── App.vue                   # Root component
├── router/                   # Vue Router configuration
│   └── index.ts
│
├── stores/                   # Pinia state management
│   ├── auth.ts              # Authentication store
│   ├── containers.ts        # Container management
│   └── berths.ts           # Berth operations
│
├── views/                    # Page components
│   ├── Dashboard.vue
│   ├── LoginView.vue
│   ├── ContainerManagement.vue
│   └── BerthOperations.vue
│
├── components/               # Reusable components
│   ├── common/              # Generic components
│   │   ├── BaseModal.vue
│   │   ├── BaseTable.vue
│   │   └── LoadingSpinner.vue
│   │
│   ├── containers/          # Container-specific components
│   │   ├── ContainerCard.vue
│   │   └── ContainerForm.vue
│   │
│   └── berths/             # Berth-specific components
│       ├── BerthCard.vue
│       └── BerthAssignmentForm.vue
│
├── services/                # API services
│   ├── api.ts              # Base API configuration
│   ├── auth.service.ts     # Authentication API
│   ├── container.service.ts
│   └── berth.service.ts
│
├── composables/             # Vue composition functions
│   ├── useAuth.ts
│   ├── useNotifications.ts
│   └── usePermissions.ts
│
├── types/                   # TypeScript type definitions
│   ├── api.types.ts
│   ├── auth.types.ts
│   └── container.types.ts
│
└── assets/                  # Static assets
    ├── images/
    └── styles/
```

## 🔧 Development Workflow

### Branch Strategy
```bash
# Feature development
git checkout -b feature/container-search
git checkout -b bugfix/auth-token-refresh
git checkout -b hotfix/security-patch

# Branch naming conventions
feature/    # New features
bugfix/     # Bug fixes
hotfix/     # Critical production fixes
chore/      # Maintenance tasks
docs/       # Documentation updates
```

### Code Standards

#### C# Coding Standards
```csharp
// Use async/await pattern
public async Task<ApiResponse<Container>> GetContainerAsync(int id)
{
    try
    {
        var container = await _repository.GetByIdAsync(id);
        if (container == null)
        {
            return ApiResponse<Container>.Failure("Container not found", 404);
        }
        
        return ApiResponse<Container>.Success(container);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving container {ContainerId}", id);
        return ApiResponse<Container>.Failure("Internal server error", 500);
    }
}

// Use dependency injection
public class ContainerController : ControllerBase
{
    private readonly IContainerService _containerService;
    private readonly ILogger<ContainerController> _logger;
    
    public ContainerController(
        IContainerService containerService,
        ILogger<ContainerController> logger)
    {
        _containerService = containerService;
        _logger = logger;
    }
}
```

#### TypeScript/Vue Coding Standards
```typescript
// Use Composition API with TypeScript
export default defineComponent({
  name: 'ContainerCard',
  props: {
    container: {
      type: Object as PropType<Container>,
      required: true
    }
  },
  emits: {
    edit: (containerId: number) => true,
    delete: (containerId: number) => true
  },
  setup(props, { emit }) {
    const isLoading = ref(false)
    
    const handleEdit = () => {
      emit('edit', props.container.id)
    }
    
    return {
      isLoading,
      handleEdit
    }
  }
})

// Use composables for reusable logic
export function useContainers() {
  const containers = ref<Container[]>([])
  const loading = ref(false)
  
  const fetchContainers = async () => {
    loading.value = true
    try {
      const response = await containerService.getContainers()
      containers.value = response.data
    } catch (error) {
      console.error('Failed to fetch containers:', error)
    } finally {
      loading.value = false
    }
  }
  
  return {
    containers: readonly(containers),
    loading: readonly(loading),
    fetchContainers
  }
}
```

### Testing Guidelines

#### Backend Testing
```csharp
// Unit test example
[Test]
public async Task GetContainer_WhenContainerExists_ReturnsContainer()
{
    // Arrange
    var containerId = 1;
    var expectedContainer = new Container { Id = containerId, ContainerNumber = "CONT001" };
    _mockRepository.Setup(x => x.GetByIdAsync(containerId))
               .ReturnsAsync(expectedContainer);
    
    // Act
    var result = await _containerService.GetContainerAsync(containerId);
    
    // Assert
    Assert.That(result.Success, Is.True);
    Assert.That(result.Data.ContainerNumber, Is.EqualTo("CONT001"));
}

// Integration test example
[Test]
public async Task CreateContainer_WithValidData_ReturnsCreatedContainer()
{
    // Arrange
    var createDto = new CreateContainerDto
    {
        ContainerNumber = "TEST001",
        Type = "DRY",
        Size = "40FT"
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/containers", createDto);
    
    // Assert
    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    var container = await response.Content.ReadFromJsonAsync<Container>();
    Assert.That(container.ContainerNumber, Is.EqualTo("TEST001"));
}
```

#### Frontend Testing
```typescript
// Component test with Vue Test Utils
describe('ContainerCard.vue', () => {
  it('emits edit event when edit button is clicked', async () => {
    const container = { id: 1, containerNumber: 'CONT001' }
    const wrapper = mount(ContainerCard, {
      props: { container }
    })
    
    await wrapper.find('[data-testid="edit-button"]').trigger('click')
    
    expect(wrapper.emitted('edit')).toBeTruthy()
    expect(wrapper.emitted('edit')[0]).toEqual([1])
  })
})

// API service test
describe('ContainerService', () => {
  it('fetches containers successfully', async () => {
    const mockContainers = [{ id: 1, containerNumber: 'CONT001' }]
    vi.mocked(api.get).mockResolvedValue({ data: mockContainers })
    
    const result = await containerService.getContainers()
    
    expect(result).toEqual(mockContainers)
    expect(api.get).toHaveBeenCalledWith('/containers')
  })
})
```

## 🔍 Debugging & Troubleshooting

### Backend Debugging
```csharp
// Enable detailed logging in appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information",
      "System.Net.Http.HttpClient": "Information"
    }
  }
}

// Use breakpoints and watch expressions in Visual Studio/VS Code
// Add structured logging for debugging
_logger.LogDebug("Processing container creation for {ContainerNumber}", dto.ContainerNumber);
```

### Frontend Debugging
```typescript
// Vue DevTools browser extension
// Enable development mode
app.config.devtools = true

// Use console logging strategically
console.log('Container data:', container)
console.debug('API response:', response)

// Debug composables
export function useContainers() {
  const containers = ref<Container[]>([])
  
  watch(containers, (newValue) => {
    console.debug('Containers updated:', newValue.length)
  })
  
  return { containers }
}
```

### Common Issues & Solutions

#### Database Connection Issues
```bash
# Check PostgreSQL status
sudo systemctl status postgresql

# Test connection
psql -h localhost -U postgres -d container_tracking_dev

# Reset migrations if needed
dotnet ef database drop
dotnet ef database update
```

#### CORS Issues
```csharp
// Ensure proper CORS configuration in Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

#### NPM/Node Issues
```bash
# Clear npm cache
npm cache clean --force

# Delete node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Check Node version
node --version  # Should be 20.x LTS
```

## 📊 Performance Optimization

### Backend Performance
```csharp
// Use async/await properly
public async Task<IActionResult> GetContainers([FromQuery] FilterDto filter)
{
    var containers = await _containerService.GetContainersAsync(filter);
    return Ok(containers);
}

// Implement caching
[ResponseCache(Duration = 300)] // 5 minutes
public async Task<IActionResult> GetPorts()
{
    var ports = await _portService.GetPortsAsync();
    return Ok(ports);
}

// Use pagination for large datasets
public async Task<PagedResult<Container>> GetContainersAsync(int page, int size)
{
    return await _repository.GetPagedAsync(page, size);
}
```

### Frontend Performance
```typescript
// Use lazy loading for routes
const routes = [
  {
    path: '/containers',
    component: () => import('./views/ContainerManagement.vue')
  }
]

// Implement virtual scrolling for large lists
import { FixedSizeList } from 'vue-virtual-scroll-list'

// Use computed properties for expensive operations
const filteredContainers = computed(() => {
  return containers.value.filter(container => 
    container.containerNumber.includes(searchTerm.value)
  )
})
```

## 🔐 Security Best Practices

### Backend Security
```csharp
// Input validation
public class CreateContainerDto
{
    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string ContainerNumber { get; set; }
    
    [Required]
    [RegularExpression(@"^(DRY|REEFER|TANK|OPEN_TOP)$")]
    public string Type { get; set; }
}

// Authorization
[Authorize(Roles = "Admin,PortManager")]
public async Task<IActionResult> DeleteContainer(int id)
{
    // Implementation
}

// Secure headers middleware
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        
        await next(context);
    }
}
```

### Frontend Security
```typescript
// Sanitize user input
import DOMPurify from 'dompurify'

const sanitizedHtml = DOMPurify.sanitize(userInput)

// Secure API calls
const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 10000,
  withCredentials: true
})

// Handle authentication tokens securely
export function useAuth() {
  const token = ref<string | null>(localStorage.getItem('authToken'))
  
  const logout = () => {
    localStorage.removeItem('authToken')
    token.value = null
    router.push('/login')
  }
  
  return { token, logout }
}
```

## 📚 Learning Resources

### Essential Documentation
- **.NET 8 Documentation**: https://docs.microsoft.com/en-us/aspnet/core
- **Vue.js 3 Guide**: https://vuejs.org/guide/
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **PostgreSQL Documentation**: https://www.postgresql.org/docs/
- **Tailwind CSS**: https://tailwindcss.com/docs

### Recommended Courses
- **C# and .NET Fundamentals**: Microsoft Learn
- **Vue.js Complete Guide**: Vue Mastery
- **Database Design**: Khan Academy
- **API Design Best Practices**: REST API Tutorial

### Community Resources
- **Stack Overflow**: For specific technical questions
- **GitHub Discussions**: Project-related discussions
- **Discord/Slack**: Team communication channels

## 🚀 Deployment & CI/CD

### Development Deployment
```bash
# Build backend for development
cd backend
dotnet build --configuration Debug

# Build frontend for development
cd frontend
npm run build:dev
```

### Production Deployment
```yaml
# GitHub Actions workflow example
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
          
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '20'
          
      - name: Build backend
        run: |
          cd backend
          dotnet restore
          dotnet build --configuration Release
          
      - name: Build frontend
        run: |
          cd frontend
          npm ci
          npm run build
          
      - name: Deploy to Azure
        # Azure deployment steps
```

## 🤝 Contributing Guidelines

### Pull Request Process
1. **Branch Creation**: Create feature branch from `develop`
2. **Development**: Implement feature with tests
3. **Testing**: Ensure all tests pass locally
4. **Code Review**: Submit PR for team review
5. **CI/CD**: Automated testing and deployment
6. **Merge**: Squash and merge to `develop`

### Code Review Checklist
- [ ] Code follows project conventions
- [ ] Tests are included and passing
- [ ] Documentation is updated
- [ ] Security considerations addressed
- [ ] Performance impact assessed
- [ ] Breaking changes documented

### Git Commit Messages
```bash
# Format: <type>(<scope>): <subject>
feat(auth): add JWT token refresh functionality
fix(containers): resolve search filter bug
docs(api): update API documentation
test(berths): add integration tests for berth assignment
refactor(services): optimize container service queries
```

## 🆘 Getting Help

### Internal Resources
- **Technical Lead**: [Email/Slack contact]
- **Architecture Team**: [Team channel/email]
- **DevOps Team**: [Contact for deployment issues]

### Documentation
- **API Documentation**: http://localhost:5266/swagger
- **Architecture Decisions**: `/docs/architecture/`
- **Troubleshooting Guide**: `/docs/TROUBLESHOOTING.md`

### Emergency Contacts
- **Production Issues**: [On-call rotation contact]
- **Security Incidents**: [Security team contact]
- **Infrastructure Issues**: [DevOps team contact]

Welcome to the team! This guide should help you get started quickly. Don't hesitate to reach out if you need any assistance or have questions about the development process.