# Quick Start Guide - Authentication System

## 🚀 Getting Started with Authentication

The Container Tracking & Port Operations System now includes a comprehensive Role-Based Access Control (RBAC) system. Follow this guide to get started quickly.

## Prerequisites

1. ✅ Backend API running (`dotnet run` in `/backend` folder)
2. ✅ Database migrations applied (done automatically on startup)
3. ✅ Environment variables configured (see [development-setup-guide.md](development-setup-guide.md))

## Step 1: Login with Default Admin Account

### Default Credentials
- **Username:** `admin`
- **Password:** `Admin123!`

### Login Request
```bash
curl -X POST http://localhost:5221/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

### Expected Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires": "2025-09-30T12:00:00Z",
  "user": {
    "userId": 1,
    "username": "admin",
    "email": "admin@containertrack.com",
    "fullName": "System Administrator",
    "roles": ["Admin"],
    "permissions": ["GlobalPortAccess", "ManageAllPorts", "..."],
    "isActive": true
  }
}
```

## Step 2: Use JWT Token for API Requests

Copy the `token` from the login response and include it in the Authorization header:

```bash
curl -X GET http://localhost:5221/api/containers \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## Step 3: Change Default Password (IMPORTANT)

**Security Note:** Change the default password immediately after first login!

```bash
curl -X POST http://localhost:5221/api/auth/change-password \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "Admin123!",
    "newPassword": "YourNewSecurePassword123!"
  }'
```

## Step 4: Create Additional Users

### Register a New Operator
```bash
curl -X POST http://localhost:5221/api/auth/register \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "operator1",
    "email": "operator1@example.com",
    "password": "SecurePassword123!",
    "fullName": "Port Operator One",
    "department": "Operations",
    "portId": 1,
    "roles": ["Operator"]
  }'
```

### Register a Viewer
```bash
curl -X POST http://localhost:5221/api/auth/register \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "viewer1",
    "email": "viewer1@example.com",
    "password": "SecurePassword123!",
    "fullName": "Port Viewer One",
    "department": "Reporting",
    "roles": ["Viewer"]
  }'
```

## Step 5: Test Role-Based Access

### Test as Admin (Full Access)
```bash
# Can create containers
curl -X POST http://localhost:5221/api/containers \
  -H "Authorization: Bearer ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "containerId": "TEST123",
    "name": "Test Container",
    "type": "Dry",
    "status": "Available",
    "currentLocation": "Port of Copenhagen",
    "size": "40ft",
    "weight": 25000.0,
    "shipId": 1
  }'
```

### Test as Viewer (Read-Only)
```bash
# Can view containers
curl -X GET http://localhost:5221/api/containers \
  -H "Authorization: Bearer VIEWER_TOKEN"

# Cannot create containers (will get 403 Forbidden)
curl -X POST http://localhost:5221/api/containers \
  -H "Authorization: Bearer VIEWER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"containerId": "WILL_FAIL"}'
```

## Available Roles & Permissions

| Role | Can Create | Can Update | Can Delete | Can View | Special Access |
|------|------------|------------|------------|----------|----------------|
| **Admin** | ✅ Everything | ✅ Everything | ✅ Everything | ✅ Everything | User management, All ports |
| **PortManager** | ✅ Port operations | ✅ Port operations | ✅ Port operations | ✅ Everything | Assigned port only |
| **Operator** | ✅ Daily operations | ✅ Daily operations | ❌ Limited | ✅ Everything | Port-specific |
| **Viewer** | ❌ No | ❌ No | ❌ No | ✅ Everything | Read-only access |

## Quick Testing with Postman

1. **Import Collection:** Use the provided Postman collection
   - File: `docs/Container-Tracking-API-Auth.postman_collection.json`

2. **Login:** Use the "Login (Admin)" request
   - JWT token is automatically saved to collection variable

3. **Test Endpoints:** All subsequent requests will use the saved token

## Swagger UI with Authentication

1. **Open Swagger:** http://localhost:5221/swagger
2. **Authorize:** Click the "Authorize" button (🔒)
3. **Enter Token:** Paste your JWT token (without "Bearer " prefix)
4. **Test APIs:** All protected endpoints are now accessible

## Common Issues & Solutions

### Issue: 401 Unauthorized
**Cause:** Missing or invalid JWT token
**Solution:** 
1. Login again to get a fresh token
2. Check token expiration (default: 60 minutes)
3. Ensure token is in Authorization header: `Bearer YOUR_TOKEN`

### Issue: 403 Forbidden
**Cause:** User lacks required permissions
**Solution:**
1. Check user roles: `GET /api/auth/profile`
2. Assign required roles (Admin only)
3. Verify endpoint permissions in [authentication-guide.md](authentication-guide.md)

### Issue: Token Expired
**Cause:** JWT token has exceeded expiration time
**Solution:**
1. Login again to get a new token
2. Consider extending token lifetime via `JWT_EXPIRATION_MINUTES` environment variable

## Environment Configuration

### Required Environment Variables
```bash
# JWT Configuration
JWT_KEY=your-secret-signing-key-minimum-32-characters-long
DB_PASSWORD=your-database-password

# Optional (with defaults)
JWT_ISSUER=ContainerTrackingAPI
JWT_AUDIENCE=ContainerTrackingClients
JWT_EXPIRATION_MINUTES=60
```

### Database Configuration
```bash
# Database Connection
DB_HOST=localhost
DB_PORT=5433
DB_NAME=ContainerTrackingDB
DB_USER=postgres
DB_PASSWORD=your-password
```

## Next Steps

1. **Explore the API:** Try different endpoints with different user roles
2. **Frontend Integration:** Use JWT tokens in your Vue.js application
3. **Custom Roles:** Create additional roles with specific permission sets
4. **Port Assignment:** Assign users to specific ports for port-specific access control

## Documentation Resources

- 📖 **[Complete Authentication Guide](authentication-guide.md)** - Detailed RBAC documentation
- 🔧 **[API Specification](api-specification.md)** - Complete API reference with auth
- 🗄️ **[Database Schema](database-entity-relationships.md)** - Authentication tables documentation
- 🧪 **[Testing Guide](testing_guide.md)** - Comprehensive testing examples

## Support

If you encounter any issues:
1. Check the application logs in the terminal
2. Verify environment variables are set correctly
3. Ensure database is running and accessible
4. Review the complete documentation guides linked above