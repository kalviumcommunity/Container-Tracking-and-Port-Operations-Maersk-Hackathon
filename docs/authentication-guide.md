# Authentication & Authorization Guide

## Overview

The Container Tracking & Port Operations System implements a comprehensive **Role-Based Access Control (RBAC)** system using JWT (JSON Web Tokens) for authentication and authorization.

## Authentication Flow

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