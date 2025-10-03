# Authentication & Authorization Guide

## Overview

The Container Tracking & Port Operations System implements a comprehensive **Role-Based Access Control (RBAC)** system using JWT (JSON Web Tokens) for authentication and authorization.

## Authentication Flow

### JWT Authentication Process

```mermaid
sequenceDiagram
    participant User
    participant Frontend
    participant API
    participant Database
    participant JWTService
    
    User->>Frontend: Enter Credentials
    Frontend->>API: POST /api/auth/login<br/>{username, password}
    API->>Database: Query User by Username
    Database-->>API: Return User + Roles
    API->>API: Verify Password Hash
    
    alt Valid Credentials
        API->>Database: Get User Roles & Permissions
        Database-->>API: Return RBAC Data
        API->>JWTService: Generate JWT Token<br/>(Claims: UserId, Roles, Permissions)
        JWTService-->>API: Return Signed Token
        API-->>Frontend: 200 OK + JWT Token + User Info
        Frontend->>Frontend: Store Token (localStorage)
        Frontend-->>User: Login Success
    else Invalid Credentials
        API-->>Frontend: 401 Unauthorized
        Frontend-->>User: Login Failed
    end
    
    Note over Frontend,API: Subsequent Requests
    Frontend->>API: Request + Authorization Header<br/>Bearer {token}
    API->>API: Validate JWT Signature
    API->>API: Check Token Expiration
    API->>API: Verify Permissions
    API-->>Frontend: Response (if authorized)
```

### Authorization Flow

```mermaid
flowchart TD
    START([API Request with JWT]) --> VALIDATE{Validate JWT<br/>Signature?}
    
    VALIDATE -->|Invalid| RETURN401[Return 401<br/>Unauthorized]
    VALIDATE -->|Valid| EXPIRED{Token<br/>Expired?}
    
    EXPIRED -->|Yes| RETURN401
    EXPIRED -->|No| EXTRACT[Extract Claims:<br/>UserId, Roles, Permissions]
    
    EXTRACT --> CHECK_PERM{Required<br/>Permission<br/>Specified?}
    
    CHECK_PERM -->|No| ALLOW[Allow Access]
    CHECK_PERM -->|Yes| HAS_PERM{User Has<br/>Permission?}
    
    HAS_PERM -->|Yes| CHECK_PORT{Port-Specific<br/>Resource?}
    HAS_PERM -->|No| RETURN403[Return 403<br/>Forbidden]
    
    CHECK_PORT -->|No| ALLOW
    CHECK_PORT -->|Yes| HAS_ACCESS{User Has<br/>GlobalPortAccess<br/>OR<br/>Assigned Port?}
    
    HAS_ACCESS -->|Yes| ALLOW
    HAS_ACCESS -->|No| RETURN403
    
    ALLOW --> EXECUTE[Execute API Method]
    EXECUTE --> SUCCESS[Return 200/201<br/>Success]
    
    style START fill:#42b983
    style ALLOW fill:#90EE90
    style SUCCESS fill:#32CD32
    style RETURN401 fill:#FF6B6B
    style RETURN403 fill:#FFA07A
```

### 1. User Registration
New users can be registered with initial roles and port assignments.

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe",
  "phoneNumber": "+1234567890",
  "department": "Operations",
  "portId": 1,
  "roles": ["Operator"]
}
```

### 2. User Login
Users authenticate using username/email and password to receive a JWT token.

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2025-09-30T12:00:00Z",
  "user": {
    "userId": 1,
    "username": "admin",
    "email": "admin@example.com",
    "fullName": "System Administrator",
    "roles": ["Admin"],
    "permissions": ["GlobalPortAccess", "ManageAllPorts", "ManageUsers", "..."],
    "assignedPort": null,
    "isActive": true
  }
}
```

### 3. Using JWT Tokens
Include the JWT token in the Authorization header for all authenticated requests:

```http
GET /api/containers
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Role-Based Access Control (RBAC)

### Default Roles

| Role | Description | Typical Use Case |
|------|-------------|------------------|
| **Admin** | Full system access | System administrators |
| **Port Manager** | Manage specific port operations | Port facility managers |
| **Operator** | Day-to-day operations | Port workers, logistics coordinators |
| **Viewer** | Read-only access | Auditors, reporting staff |

### Permission Categories

#### System Management
- `GlobalPortAccess` - Access all ports in the system
- `ManageAllPorts` - Create, update, delete any port
- `ManageUsers` - User account management
- `ManageRoles` - Role and permission management
- `ViewSystemReports` - System-wide reporting

#### Port Operations
- `ManagePortDetails` - Modify port information
- `ViewPortDetails` - View port information
- `ViewPortReports` - Port-specific reports

#### Container Management
- `ManageContainers` - Create, update, delete containers
- `ViewContainers` - View container information
- `TrackContainers` - Container tracking operations

#### Ship Operations
- `ManageShips` - Ship management operations
- `ViewShips` - View ship information
- `ScheduleShips` - Ship scheduling

#### Cargo Operations
- `ManageCargo` - Cargo management
- `ViewCargo` - View cargo information

#### Berth Management
- `ManageBerths` - Berth allocation and management
- `ViewBerths` - View berth information
- `AllocateBerths` - Berth allocation operations

#### Equipment Management
- `ManageEquipment` - Equipment management
- `ViewEquipment` - View equipment information

### Role-Permission Matrix

```mermaid
graph TD
    subgraph "Admin Role - Full System Access"
        ADMIN[Admin Role]
        ADMIN --> SYS_PERMS[System Permissions]
        SYS_PERMS --> GP[GlobalPortAccess]
        SYS_PERMS --> MAP[ManageAllPorts]
        SYS_PERMS --> MU[ManageUsers]
        SYS_PERMS --> MR[ManageRoles]
        SYS_PERMS --> VSR[ViewSystemReports]
        
        ADMIN --> ALL_MGMT[All Management Permissions]
        ALL_MGMT --> MPD[ManagePortDetails]
        ALL_MGMT --> MC[ManageContainers]
        ALL_MGMT --> MS[ManageShips]
        ALL_MGMT --> MCG[ManageCargo]
        ALL_MGMT --> MB[ManageBerths]
        ALL_MGMT --> ME[ManageEquipment]
        ALL_MGMT --> SS[ScheduleShips]
        ALL_MGMT --> AB[AllocateBerths]
        
        ADMIN --> ALL_VIEW[All View Permissions]
        ALL_VIEW --> VPD[ViewPortDetails]
        ALL_VIEW --> VC[ViewContainers]
        ALL_VIEW --> VS[ViewShips]
        ALL_VIEW --> VCG[ViewCargo]
        ALL_VIEW --> VB[ViewBerths]
        ALL_VIEW --> VE[ViewEquipment]
        ALL_VIEW --> VPR[ViewPortReports]
        ALL_VIEW --> TC[TrackContainers]
    end
    
    subgraph "Port Manager - Port Operations"
        PM[Port Manager Role]
        PM --> PM_MGMT[Management Permissions]
        PM_MGMT --> PM_MPD[ManagePortDetails]
        PM_MGMT --> PM_MC[ManageContainers]
        PM_MGMT --> PM_MS[ManageShips]
        PM_MGMT --> PM_MCG[ManageCargo]
        PM_MGMT --> PM_MB[ManageBerths]
        PM_MGMT --> PM_ME[ManageEquipment]
        PM_MGMT --> PM_SS[ScheduleShips]
        PM_MGMT --> PM_AB[AllocateBerths]
        
        PM --> PM_VIEW[View Permissions]
        PM_VIEW --> PM_VPD[ViewPortDetails]
        PM_VIEW --> PM_VC[ViewContainers]
        PM_VIEW --> PM_VS[ViewShips]
        PM_VIEW --> PM_VCG[ViewCargo]
        PM_VIEW --> PM_VB[ViewBerths]
        PM_VIEW --> PM_VE[ViewEquipment]
        PM_VIEW --> PM_VPR[ViewPortReports]
        PM_VIEW --> PM_TC[TrackContainers]
    end
    
    subgraph "Operator - Daily Operations"
        OP[Operator Role]
        OP --> OP_MGMT[Operational Permissions]
        OP_MGMT --> OP_MC[ManageContainers]
        OP_MGMT --> OP_MS[ManageShips]
        OP_MGMT --> OP_MCG[ManageCargo]
        OP_MGMT --> OP_MB[ManageBerths]
        OP_MGMT --> OP_AB[AllocateBerths]
        
        OP --> OP_VIEW[View Permissions]
        OP_VIEW --> OP_VPD[ViewPortDetails]
        OP_VIEW --> OP_VC[ViewContainers]
        OP_VIEW --> OP_VS[ViewShips]
        OP_VIEW --> OP_VCG[ViewCargo]
        OP_VIEW --> OP_VB[ViewBerths]
        OP_VIEW --> OP_VE[ViewEquipment]
        OP_VIEW --> OP_VPR[ViewPortReports]
        OP_VIEW --> OP_TC[TrackContainers]
    end
    
    subgraph "Viewer - Read Only"
        VW[Viewer Role]
        VW --> VW_VIEW[View-Only Permissions]
        VW_VIEW --> VW_VPD[ViewPortDetails]
        VW_VIEW --> VW_VC[ViewContainers]
        VW_VIEW --> VW_VS[ViewShips]
        VW_VIEW --> VW_VCG[ViewCargo]
        VW_VIEW --> VW_VB[ViewBerths]
        VW_VIEW --> VW_VE[ViewEquipment]
        VW_VIEW --> VW_VPR[ViewPortReports]
        VW_VIEW --> VW_TC[TrackContainers]
    end
    
    style ADMIN fill:#FF6B6B
    style PM fill:#4ECDC4
    style OP fill:#45B7D1
    style VW fill:#95E1D3
```

| Permission | Admin | Port Manager | Operator | Viewer |
|------------|-------|--------------|----------|--------|
| GlobalPortAccess | ✅ | ❌ | ❌ | ❌ |
| ManageAllPorts | ✅ | ❌ | ❌ | ❌ |
| ManageUsers | ✅ | ❌ | ❌ | ❌ |
| ManageRoles | ✅ | ❌ | ❌ | ❌ |
| ViewSystemReports | ✅ | ❌ | ❌ | ❌ |
| ManagePortDetails | ✅ | ✅ | ❌ | ❌ |
| ViewPortDetails | ✅ | ✅ | ✅ | ✅ |
| ViewPortReports | ✅ | ✅ | ✅ | ✅ |
| ManageContainers | ✅ | ✅ | ✅ | ❌ |
| ViewContainers | ✅ | ✅ | ✅ | ✅ |
| TrackContainers | ✅ | ✅ | ✅ | ✅ |
| ManageShips | ✅ | ✅ | ✅ | ❌ |
| ViewShips | ✅ | ✅ | ✅ | ✅ |
| ScheduleShips | ✅ | ✅ | ❌ | ❌ |
| ManageCargo | ✅ | ✅ | ✅ | ❌ |
| ViewCargo | ✅ | ✅ | ✅ | ✅ |
| ManageBerths | ✅ | ✅ | ✅ | ❌ |
| ViewBerths | ✅ | ✅ | ✅ | ✅ |
| AllocateBerths | ✅ | ✅ | ✅ | ❌ |
| ManageEquipment | ✅ | ✅ | ❌ | ❌ |
| ViewEquipment | ✅ | ✅ | ✅ | ✅ |

## Authorization Attributes

The API uses custom authorization attributes to protect endpoints:

### `[RequirePermission]`
Requires specific permissions to access an endpoint.

```csharp
[RequirePermission("ManageContainers")]
[HttpPost]
public async Task<IActionResult> CreateContainer([FromBody] CreateContainerDto dto)
```

### `[RequireRole]`
Requires specific roles to access an endpoint.

```csharp
[RequireRole("Admin", "PortManager")]
[HttpGet("admin/users")]
public async Task<IActionResult> GetAllUsers()
```

### `[RequirePortAccess]`
Requires access to a specific port.

```csharp
[RequirePortAccess]
[HttpGet("port/{portId}/containers")]
public async Task<IActionResult> GetPortContainers(int portId)
```

### `[RequireOwnership]`
Requires ownership of a resource or admin privileges.

```csharp
[RequireOwnership]
[HttpPut("users/{userId}")]
public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserDto dto)
```

## Default Admin Account

### Credentials
- **Username:** `admin`
- **Password:** `Admin123!`
- **Email:** `admin@containertrack.com`
- **Role:** Admin (all permissions)

### First Login Steps
1. Login with default credentials
2. **Change the default password immediately**
3. Create additional user accounts as needed
4. Assign appropriate roles and port access

## User Management API

### Get Current User Profile
```http
GET /api/auth/profile
Authorization: Bearer {token}
```

### Change Password
```http
POST /api/auth/change-password
Authorization: Bearer {token}
Content-Type: application/json

{
  "currentPassword": "OldPassword123!",
  "newPassword": "NewPassword123!"
}
```

### Assign Roles (Admin Only)
```http
POST /api/auth/users/{userId}/roles
Authorization: Bearer {token}
Content-Type: application/json

{
  "roleNames": ["Operator", "Viewer"]
}
```

### Remove Roles (Admin Only)
```http
DELETE /api/auth/users/{userId}/roles
Authorization: Bearer {token}
Content-Type: application/json

{
  "roleNames": ["Viewer"]
}
```

## JWT Token Details

### Token Structure
```json
{
  "sub": "1",
  "username": "admin",
  "email": "admin@containertrack.com",
  "roles": ["Admin"],
  "permissions": ["GlobalPortAccess", "ManageAllPorts", "..."],
  "portId": null,
  "iat": 1696089600,
  "exp": 1696093200,
  "iss": "ContainerTrackingAPI",
  "aud": "ContainerTrackingClients"
}
```

### Token Expiration
- **Default Expiration:** 60 minutes
- **Configurable:** Via `JWT_EXPIRATION_MINUTES` environment variable
- **Auto-refresh:** Not implemented (client should handle re-authentication)

## Security Best Practices

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

### Token Security
- Store JWT tokens securely (e.g., httpOnly cookies)
- Never expose tokens in URLs or logs
- Implement token refresh mechanisms
- Use HTTPS in production

### Environment Variables
```bash
# Required
JWT_KEY=your-secret-key-here-minimum-32-characters
DB_PASSWORD=your-database-password

# Optional (with defaults)
JWT_ISSUER=ContainerTrackingAPI
JWT_AUDIENCE=ContainerTrackingClients
JWT_EXPIRATION_MINUTES=60
```

## Error Responses

### Authentication Errors
```json
{
  "success": false,
  "message": "Invalid credentials",
  "timestamp": "2025-09-30T10:00:00Z"
}
```

### Authorization Errors
```json
{
  "success": false,
  "message": "Insufficient permissions",
  "timestamp": "2025-09-30T10:00:00Z"
}
```

### Token Expired
```json
{
  "success": false,
  "message": "Token has expired",
  "timestamp": "2025-09-30T10:00:00Z"
}
```

## Frontend Integration

### Vue.js Example
```javascript
// Login function
async function login(credentials) {
  const response = await fetch('/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(credentials)
  });
  
  const data = await response.json();
  
  if (data.token) {
    localStorage.setItem('authToken', data.token);
    localStorage.setItem('user', JSON.stringify(data.user));
  }
  
  return data;
}

// API call with authentication
async function fetchContainers() {
  const token = localStorage.getItem('authToken');
  
  const response = await fetch('/api/containers', {
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });
  
  return response.json();
}
```

## Testing Authentication

Use the provided Postman collection or test with curl:

```bash
# Login
curl -X POST http://localhost:5221/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'

# Use token for authenticated request
curl -X GET http://localhost:5221/api/containers \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```