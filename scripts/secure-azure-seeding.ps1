# Secure Azure PostgreSQL Enhanced Seeding Script
# This script uses the API endpoints securely for seeding data

Write-Host "🚀 Secure Azure PostgreSQL Enhanced Seeding..." -ForegroundColor Green
Write-Host ""

# Step 1: Check environment
Write-Host "1. Environment Check..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "✅ .env file exists" -ForegroundColor Green
} else {
    Write-Host "❌ .env file not found. Please ensure .env is configured." -ForegroundColor Red
    exit 1
}

# Step 2: Start API
Write-Host ""
Write-Host "2. Starting API server..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -NoNewWindow -PassThru
Start-Sleep -Seconds 10

try {
    # Wait for API to be ready
    $apiReady = $false
    $attempts = 0
    $maxAttempts = 30
    
    Write-Host "   Waiting for API to start..." -ForegroundColor Cyan
    while (-not $apiReady -and $attempts -lt $maxAttempts) {
        try {
            $null = Invoke-RestMethod -Uri "http://localhost:5221/api/health" -Method GET -TimeoutSec 5
            $apiReady = $true
            Write-Host "✅ API is ready!" -ForegroundColor Green
        }
        catch {
            $attempts++
            Write-Host "   ⏳ Attempt $attempts/$maxAttempts..." -ForegroundColor Yellow
            Start-Sleep -Seconds 2
        }
    }
    
    if (-not $apiReady) {
        throw "API failed to start after $maxAttempts attempts"
    }

    # Step 3: Authenticate as admin
    Write-Host ""
    Write-Host "3. Authenticating as admin..." -ForegroundColor Yellow
    $loginBody = @{
        username = "admin"
        password = "Admin123!"
    } | ConvertTo-Json
    
    try {
        $loginResponse = Invoke-RestMethod -Uri "http://localhost:5221/api/auth/login" -Method POST -Body $loginBody -ContentType "application/json"
        $token = $loginResponse.token
        Write-Host "✅ Admin authentication successful!" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Authentication failed. Ensure the admin user exists and password is correct." -ForegroundColor Red
        throw
    }

    # Step 4: Run enhanced seeding
    Write-Host ""
    Write-Host "4. 🌱 Running enhanced business data seeding..." -ForegroundColor Yellow
    Write-Host "   📍 Seeding 25 major world ports..." -ForegroundColor Cyan
    Write-Host "   🚢 Seeding 60+ ships from major shipping lines..." -ForegroundColor Cyan
    Write-Host "   📦 Seeding 300 containers with diverse cargo..." -ForegroundColor Cyan
    
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    
    try {
        $seedingResponse = Invoke-RestMethod -Uri "http://localhost:5221/api/seed/enhanced-business-data" -Method POST -Headers $headers -TimeoutSec 120
        
        if ($seedingResponse.success) {
            Write-Host "✅ Enhanced business data seeding completed successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "📊 Seeded Data Summary:" -ForegroundColor Cyan
            Write-Host "   📍 Ports: $($seedingResponse.data.ports)" -ForegroundColor White
            Write-Host "   🚢 Ships: $($seedingResponse.data.ships)" -ForegroundColor White  
            Write-Host "   📦 Containers: $($seedingResponse.data.containers)" -ForegroundColor White
            Write-Host "   ⚓ Berth Assignments: $($seedingResponse.data.berthAssignments)" -ForegroundColor White
            Write-Host "   🔗 Ship-Container Operations: $($seedingResponse.data.shipContainers)" -ForegroundColor White
        } else {
            Write-Host "⚠️ Seeding response: $($seedingResponse.message)" -ForegroundColor Yellow
        }
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 403) {
            Write-Host "❌ Access denied. Admin role required for seeding operations." -ForegroundColor Red
        } else {
            Write-Host "❌ Seeding failed: $($_.Exception.Message)" -ForegroundColor Red
        }
        throw
    }
    
    # Step 5: Get final status
    Write-Host ""
    Write-Host "5. 📊 Final Database Status:" -ForegroundColor Yellow
    try {
        $statusResponse = Invoke-RestMethod -Uri "http://localhost:5221/api/seed/status" -Method GET -Headers $headers
        
        if ($statusResponse.success) {
            $status = $statusResponse.data
            Write-Host "   📍 Total Ports: $($status.ports)" -ForegroundColor White
            Write-Host "   🚢 Total Ships: $($status.ships)" -ForegroundColor White
            Write-Host "   📦 Total Containers: $($status.containers)" -ForegroundColor White
            Write-Host "   ⚓ Total Berths: $($status.berths)" -ForegroundColor White
            Write-Host "   📋 Total Berth Assignments: $($status.berthAssignments)" -ForegroundColor White
            Write-Host "   🔗 Total Ship-Container Records: $($status.shipContainers)" -ForegroundColor White
            Write-Host "   👥 Total Users: $($status.users)" -ForegroundColor White
            Write-Host "   🔐 Total Roles: $($status.roles)" -ForegroundColor White
        }
    }
    catch {
        Write-Host "⚠️ Could not retrieve final status" -ForegroundColor Yellow
    }

    Write-Host ""
    Write-Host "🎯 Enhanced seeding completed successfully!" -ForegroundColor Green
    Write-Host "🌐 API is running at: http://localhost:5221" -ForegroundColor Cyan
    Write-Host "📚 Swagger docs: http://localhost:5221/swagger" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Press Ctrl+C to stop the API..." -ForegroundColor Gray
    
    # Keep the API running
    $apiProcess.WaitForExit()
}
catch {
    Write-Host ""
    Write-Host "❌ Error during seeding process: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # Clean up: Stop the API process
    if ($apiProcess -and !$apiProcess.HasExited) {
        Write-Host ""
        Write-Host "🛑 Stopping API server..." -ForegroundColor Yellow
        $apiProcess.Kill()
        $apiProcess.WaitForExit(5000)
    }
}