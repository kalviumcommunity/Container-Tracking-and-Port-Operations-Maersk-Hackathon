# Container Tracking API - Authentication Testing Script
# This script helps you quickly test JWT authentication and API endpoints

param(
    [string]$BaseUrl = "http://localhost:5221",
    [string]$Username = "admin",
    [string]$Password = "Admin123!"
)

Write-Host "🔐 Container Tracking API - Authentication Testing" -ForegroundColor Cyan
Write-Host "Base URL: $BaseUrl" -ForegroundColor Green
Write-Host "Username: $Username" -ForegroundColor Green
Write-Host ""

# Function to make API requests
function Invoke-ApiRequest {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [hashtable]$Headers = @{},
        [string]$Body = $null,
        [string]$Description
    )
    
    Write-Host "📡 $Description" -ForegroundColor Yellow
    Write-Host "   $Method $Url" -ForegroundColor Gray
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
        }
        
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "   ✅ Success" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Step 1: Login and get JWT token
Write-Host "Step 1: Authenticating..." -ForegroundColor Cyan
$loginData = @{
    username = $Username
    password = $Password
} | ConvertTo-Json

$authResponse = Invoke-ApiRequest -Url "$BaseUrl/api/auth/login" -Method "POST" -Body $loginData -Description "Login"

if (-not $authResponse) {
    Write-Host "❌ Authentication failed. Make sure the API is running and credentials are correct." -ForegroundColor Red
    exit 1
}

$token = $authResponse.token
$user = $authResponse.user

Write-Host ""
Write-Host "🎉 Authentication Successful!" -ForegroundColor Green
Write-Host "User ID: $($user.id)" -ForegroundColor White
Write-Host "Username: $($user.username)" -ForegroundColor White
Write-Host "Email: $($user.email)" -ForegroundColor White
Write-Host "Roles: $($user.roles -join ', ')" -ForegroundColor White
Write-Host "Token expires: $($authResponse.expiresAt)" -ForegroundColor White
Write-Host ""

# Create headers with JWT token
$headers = @{
    Authorization = "Bearer $token"
}

# Step 2: Test user profile endpoint
Write-Host "Step 2: Testing authenticated endpoints..." -ForegroundColor Cyan
$profile = Invoke-ApiRequest -Url "$BaseUrl/api/auth/profile" -Headers $headers -Description "Get user profile"

# Step 3: Test containers endpoint
$containers = Invoke-ApiRequest -Url "$BaseUrl/api/containers" -Headers $headers -Description "Get containers"

if ($containers) {
    Write-Host "   Found $($containers.Count) containers" -ForegroundColor White
}

# Step 4: Test ports endpoint
$ports = Invoke-ApiRequest -Url "$BaseUrl/api/ports" -Headers $headers -Description "Get ports"

if ($ports) {
    Write-Host "   Found $($ports.Count) ports" -ForegroundColor White
}

# Step 5: Test ships endpoint
$ships = Invoke-ApiRequest -Url "$BaseUrl/api/ships" -Headers $headers -Description "Get ships"

if ($ships) {
    Write-Host "   Found $($ships.Count) ships" -ForegroundColor White
}

# Step 6: Test admin-only endpoint (if admin user)
if ($user.roles -contains "Admin") {
    Write-Host ""
    Write-Host "Step 3: Testing admin endpoints..." -ForegroundColor Cyan
    $users = Invoke-ApiRequest -Url "$BaseUrl/api/auth/users" -Headers $headers -Description "Get all users (Admin only)"
    
    if ($users) {
        Write-Host "   Found $($users.Count) users in system" -ForegroundColor White
    }
}

Write-Host ""
Write-Host "🎯 Testing Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "💡 Tips:" -ForegroundColor Yellow
Write-Host "   - Save the JWT token for use in other tools:"
Write-Host "     `$env:JWT_TOKEN = '$token'" -ForegroundColor Gray
Write-Host "   - Use the token in curl commands:"
Write-Host "     curl -H 'Authorization: Bearer $token' $BaseUrl/api/containers" -ForegroundColor Gray
Write-Host "   - Import the Postman collection for easier testing:"
Write-Host "     docs/Container-Tracking-API-Auth.postman_collection.json" -ForegroundColor Gray
Write-Host ""

# Export token to environment variable
$env:JWT_TOKEN = $token
Write-Host "🔑 JWT token exported to `$env:JWT_TOKEN environment variable" -ForegroundColor Green