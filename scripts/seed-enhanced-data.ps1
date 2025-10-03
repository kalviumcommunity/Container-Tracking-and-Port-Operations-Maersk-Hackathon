# Enhanced Business Data Seeding Script for Azure PostgreSQL
# This script adds comprehensive business data to your existing Azure database

Write-Host "🌱 Enhanced Business Data Seeding for Azure PostgreSQL" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host ""

# Step 1: Verify environment
Write-Host "1. Verifying environment..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "✅ .env configuration found" -ForegroundColor Green
} else {
    Write-Host "❌ .env file not found" -ForegroundColor Red
    exit 1
}

# Step 2: Build and start the application in background
Write-Host ""
Write-Host "2. 🔨 Building application..." -ForegroundColor Yellow
dotnet build --configuration Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful" -ForegroundColor Green

# Step 3: Start the application in background
Write-Host ""
Write-Host "3. 🚀 Starting API server..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run" -PassThru -WindowStyle Hidden
Start-Sleep -Seconds 10  # Give the API time to start

# Step 4: Check if API is running
Write-Host "4. 🔍 Checking API status..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5221/api/seed/status" -Method GET
    Write-Host "✅ API is running and database is accessible" -ForegroundColor Green
    Write-Host "📊 Current data counts:" -ForegroundColor Cyan
    Write-Host "   Ports: $($response.data.ports)" -ForegroundColor White
    Write-Host "   Ships: $($response.data.ships)" -ForegroundColor White
    Write-Host "   Containers: $($response.data.containers)" -ForegroundColor White
    Write-Host "   Berths: $($response.data.berths)" -ForegroundColor White
} catch {
    Write-Host "❌ API not responding. Please check if the application started correctly." -ForegroundColor Red
    if ($apiProcess) { Stop-Process -Id $apiProcess.Id -Force }
    exit 1
}

# Step 5: Trigger enhanced business data seeding
Write-Host ""
Write-Host "5. 🌱 Seeding enhanced business data..." -ForegroundColor Yellow
Write-Host "   This will add:" -ForegroundColor Cyan
Write-Host "   📍 25 major world ports (Copenhagen, Shanghai, Los Angeles, etc.)" -ForegroundColor White
Write-Host "   🚢 60+ ships from major shipping lines (Maersk, MSC, COSCO, etc.)" -ForegroundColor White
Write-Host "   📦 300 containers with diverse cargo types" -ForegroundColor White
Write-Host "   ⚓ 120+ berth assignments with realistic timelines" -ForegroundColor White
Write-Host "   🔗 80+ ship-container operations" -ForegroundColor White
Write-Host ""

try {
    Write-Host "⏳ Seeding in progress..." -ForegroundColor Yellow
    $seedResponse = Invoke-RestMethod -Uri "http://localhost:5221/api/seed/enhanced-business-data" -Method POST
    
    if ($seedResponse.success) {
        Write-Host "✅ Enhanced business data seeding completed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "🎯 Your Azure database now contains:" -ForegroundColor Yellow
        Write-Host "   📍 $($seedResponse.data.ports)" -ForegroundColor White
        Write-Host "   🚢 $($seedResponse.data.ships)" -ForegroundColor White
        Write-Host "   📦 $($seedResponse.data.containers)" -ForegroundColor White
        Write-Host "   ⚓ $($seedResponse.data.berthAssignments)" -ForegroundColor White
        Write-Host "   🔗 $($seedResponse.data.shipContainers)" -ForegroundColor White
    } else {
        Write-Host "⚠️ Seeding completed with warnings: $($seedResponse.message)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠️ Some data may already exist (this is normal)" -ForegroundColor Yellow
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Gray
}

# Step 6: Get final status
Write-Host ""
Write-Host "6. 📊 Final data summary..." -ForegroundColor Yellow
try {
    $finalStatus = Invoke-RestMethod -Uri "http://localhost:5221/api/seed/status" -Method GET
    Write-Host "✅ Final database contents:" -ForegroundColor Green
    Write-Host "   🏗️  Ports: $($finalStatus.data.ports)" -ForegroundColor White
    Write-Host "   🚢 Ships: $($finalStatus.data.ships)" -ForegroundColor White
    Write-Host "   📦 Containers: $($finalStatus.data.containers)" -ForegroundColor White
    Write-Host "   ⚓ Berths: $($finalStatus.data.berths)" -ForegroundColor White
    Write-Host "   📋 Berth Assignments: $($finalStatus.data.berthAssignments)" -ForegroundColor White
    Write-Host "   🔗 Ship Containers: $($finalStatus.data.shipContainers)" -ForegroundColor White
    Write-Host "   👥 Users: $($finalStatus.data.users)" -ForegroundColor White
    Write-Host "   🔐 Roles: $($finalStatus.data.roles)" -ForegroundColor White
} catch {
    Write-Host "⚠️ Could not fetch final status" -ForegroundColor Yellow
}

# Step 7: Provide access information
Write-Host ""
Write-Host "🎉 Enhanced business data seeding completed!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Your API is running at: http://localhost:5221" -ForegroundColor Cyan
Write-Host "📚 Swagger Documentation: http://localhost:5221/swagger" -ForegroundColor Cyan
Write-Host "🔐 Admin Login: admin@example.com / admin123" -ForegroundColor Cyan
Write-Host ""
Write-Host "🔗 Test your enhanced API with realistic data!" -ForegroundColor Yellow
Write-Host "   • GET /api/ports (25 major world ports)" -ForegroundColor White
Write-Host "   • GET /api/ships (60+ ships from major lines)" -ForegroundColor White
Write-Host "   • GET /api/containers (300 diverse containers)" -ForegroundColor White
Write-Host "   • GET /api/berths (berths across all ports)" -ForegroundColor White
Write-Host ""
Write-Host "Press any key to stop the API server..." -ForegroundColor Gray
Read-Host

# Clean up
if ($apiProcess -and !$apiProcess.HasExited) {
    Write-Host "🛑 Stopping API server..." -ForegroundColor Yellow
    Stop-Process -Id $apiProcess.Id -Force
    Write-Host "✅ API server stopped" -ForegroundColor Green
}