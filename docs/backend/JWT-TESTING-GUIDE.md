# 🔐 JWT Authentication Quick Test Guide

## Prerequisites
Make sure the API is running:
```bash
cd backend
dotnet run
```

## Method 1: PowerShell (Windows) 🖥️

### Step 1: Get JWT Token
```powershell
$loginData = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5221/api/auth/login" `
  -Method POST `
  -Body $loginData `
  -ContentType "application/json"

$token = $response.token
Write-Host "Token: $($token.Substring(0, 50))..."
Write-Host "User: $($response.user.username)"
Write-Host "Roles: $($response.user.roles -join ', ')"
```

### Step 2: Test Authenticated Endpoints
```powershell
$headers = @{Authorization = "Bearer $token"}

# Test endpoints
$containers = Invoke-RestMethod -Uri "http://localhost:5221/api/containers" -Headers $headers
$ports = Invoke-RestMethod -Uri "http://localhost:5221/api/ports" -Headers $headers
$ships = Invoke-RestMethod -Uri "http://localhost:5221/api/ships" -Headers $headers

Write-Host "Found $($containers.Count) containers"
Write-Host "Found $($ports.Count) ports"
Write-Host "Found $($ships.Count) ships"
```

## Method 2: cURL (Linux/Mac/Windows) 🐧

### Step 1: Get JWT Token
```bash
# Login and extract token
TOKEN=$(curl -s -X POST "http://localhost:5221/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}' | \
  grep -o '"token":"[^"]*' | cut -d'"' -f4)

echo "Token: ${TOKEN:0:50}..."
```

### Step 2: Test Authenticated Endpoints
```bash
# Test endpoints
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5221/api/containers"
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5221/api/ports"
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5221/api/ships"

# Admin-only endpoint
curl -H "Authorization: Bearer $TOKEN" "http://localhost:5221/api/auth/users"
```

## Method 3: Postman Collection 📮

1. **Import Collection**: `docs/Container-Tracking-API-Auth.postman_collection.json`
2. **Run Login**: Execute "Login (Admin)" request
3. **Auto-Token**: JWT is automatically saved to collection variable
4. **Test APIs**: All other requests will use the saved token automatically

## Method 4: Automated Scripts 🤖

### PowerShell (Full Test)
```bash
./scripts/test-auth.ps1
```

### Bash (Full Test)
```bash
./scripts/test-auth.sh
```

## Available Roles & Permissions 👥

| Role | Key Permissions | Description |
|------|----------------|-------------|
| **Admin** | All permissions | Full system access |
| **PortManager** | Port/Ship/Container management | Port-specific management |
| **Operator** | Daily operations | Container/Cargo operations |
| **Viewer** | Read-only access | View all data |

## Common Endpoints to Test 🎯

### Public (No Auth Required)
- `GET /api/health` - Health check

### Authenticated Endpoints
- `GET /api/auth/profile` - Current user profile
- `GET /api/containers` - List containers
- `GET /api/ports` - List ports
- `GET /api/ships` - List ships

### Admin-Only Endpoints
- `GET /api/auth/users` - List all users
- `POST /api/auth/register` - Create new user

### Protected Operations
- `POST /api/containers` - Create container (requires ManageContainers)
- `PUT /api/containers/{id}` - Update container (requires ManageContainers)
- `DELETE /api/containers/{id}` - Delete container (requires ManageContainers)

## Default Admin Credentials 🔑

- **Username**: `admin`
- **Password**: `Admin123!`
- **Email**: `admin@containertrack.com`
- **Roles**: Admin (all permissions)

## Troubleshooting 🔧

### API Not Running
```bash
cd backend
dotnet run
# Should show: Now listening on: http://localhost:5221
```

### Invalid Token Error
- Check token expiration (default: 24 hours)
- Re-login to get new token

### Permission Denied
- Check user roles and permissions
- Ensure correct endpoints for user role

### Connection Refused
- Verify API is running on port 5221
- Check firewall/antivirus settings

## Quick Copy-Paste Examples 📋

### PowerShell One-Liner
```powershell
$token = (Invoke-RestMethod -Uri "http://localhost:5221/api/auth/login" -Method POST -Body (@{username="admin";password="Admin123!"} | ConvertTo-Json) -ContentType "application/json").token; Invoke-RestMethod -Uri "http://localhost:5221/api/containers" -Headers @{Authorization="Bearer $token"}
```

### Bash One-Liner
```bash
TOKEN=$(curl -s -X POST "http://localhost:5221/api/auth/login" -H "Content-Type: application/json" -d '{"username":"admin","password":"Admin123!"}' | grep -o '"token":"[^"]*' | cut -d'"' -f4) && curl -H "Authorization: Bearer $TOKEN" "http://localhost:5221/api/containers"
```

## Next Steps 🚀

1. **Change Default Password**: Update admin password immediately
2. **Create Users**: Add team members with appropriate roles
3. **Test Permissions**: Verify role-based access control
4. **Integrate Frontend**: Use JWT tokens in your frontend application
5. **Production Setup**: Configure JWT secret and expiration for production

---

**📚 More Documentation**: See `docs/authentication-guide.md` for complete RBAC documentation