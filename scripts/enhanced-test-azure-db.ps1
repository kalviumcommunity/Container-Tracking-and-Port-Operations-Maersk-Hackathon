# Enhanced Azure PostgreSQL Testing Script with Comprehensive Business Data
# Run this after setting up your Azure database

Write-Host "🚀 Enhanced Azure PostgreSQL Connection & Business Data Seeding..." -ForegroundColor Green
Write-Host ""

# Step 1: Check current environment
Write-Host "1. Current Environment Check..." -ForegroundColor Yellow
if (Test-Path ".env") {
    Write-Host "✅ .env file exists" -ForegroundColor Green
} else {
    Write-Host "❌ .env file not found" -ForegroundColor Red
    exit 1
}

# Step 2: Run database migrations (if needed)
Write-Host ""
Write-Host "2. Checking database migrations..." -ForegroundColor Yellow
$migrationStatus = dotnet ef migrations list
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Database migrations verified" -ForegroundColor Green
} else {
    Write-Host "⚠️ Running database update..." -ForegroundColor Yellow
    dotnet ef database update
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Database migrations completed successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ Database migration failed" -ForegroundColor Red
        exit 1
    }
}

# Step 3: Enhanced Business Data Seeding
Write-Host ""
Write-Host "3. 🌱 Seeding Comprehensive Business Data..." -ForegroundColor Yellow
Write-Host "   📍 Seeding 25 major world ports..." -ForegroundColor Cyan
Write-Host "   🚢 Seeding 60+ ships from major shipping lines..." -ForegroundColor Cyan
Write-Host "   📦 Seeding 300 containers with diverse cargo..." -ForegroundColor Cyan
Write-Host "   ⚓ Creating realistic berth assignments..." -ForegroundColor Cyan
Write-Host "   🔗 Establishing ship-container relationships..." -ForegroundColor Cyan
Write-Host ""

# Create a temporary seeding program
$seedingCode = @"
using Backend.Data;
using Backend.Data.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DotNetEnv;

// Load environment variables
Env.Load();

var builder = Host.CreateDefaultBuilder(args);

// Configure services
builder.ConfigureServices((context, services) =>
{
    // Database configuration
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "ContainerTrackingDB";
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "";
    var dbSslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE") ?? "Prefer";
    
    // Build connection string with SSL support for Azure
    var connectionString = `$"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode={dbSslMode};Trust Server Certificate=true";
    
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
});

var app = builder.Build();

// Seed enhanced business data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting enhanced business data seeding...");
        await EnhancedDataSeeder.SeedAsync(context);
        logger.LogInformation("Enhanced business data seeding completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred during enhanced business data seeding");
        Environment.Exit(1);
    }
}

Console.WriteLine("✅ Enhanced Business Data Seeding Completed!");
"@

# Save the seeding code to a temporary file
$seedingCode | Out-File -FilePath "TempSeeder.cs" -Encoding UTF8

# Compile and run the seeding program
Write-Host "   🔨 Compiling enhanced business data seeder..." -ForegroundColor Cyan
$compileResult = dotnet run --project . -- --seed-enhanced-data 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Enhanced business data seeding completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎯 Your database now contains:" -ForegroundColor Yellow
    Write-Host "   📍 25 Major World Ports (Copenhagen, Rotterdam, Shanghai, Los Angeles, etc.)" -ForegroundColor White
    Write-Host "   🚢 60+ Ships from Major Lines (Maersk, MSC, COSCO, Evergreen, etc.)" -ForegroundColor White
    Write-Host "   📦 300 Containers with Diverse Cargo Types" -ForegroundColor White
    Write-Host "   ⚓ 120+ Berth Assignments with Realistic Timelines" -ForegroundColor White
    Write-Host "   🔗 80+ Ship-Container Operations" -ForegroundColor White
    Write-Host "   🔐 Complete Authentication System (Admin: admin@example.com / admin123)" -ForegroundColor White
} else {
    Write-Host "⚠️ Enhanced business data seeding skipped (data may already exist)" -ForegroundColor Yellow
}

# Clean up temporary file
if (Test-Path "TempSeeder.cs") {
    Remove-Item "TempSeeder.cs" -Force
}

# Step 4: Start the application
Write-Host ""
Write-Host "4. 🌐 Starting the Enhanced Container Tracking API..." -ForegroundColor Yellow
Write-Host ""
Write-Host "🎯 Your Enhanced API is now available:" -ForegroundColor Green
Write-Host "   🌐 API Base URL: http://localhost:5221" -ForegroundColor Cyan
Write-Host "   📚 Swagger Documentation: http://localhost:5221/swagger" -ForegroundColor Cyan
Write-Host "   🔐 Login Endpoint: POST http://localhost:5221/api/auth/login" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Test with realistic data:" -ForegroundColor Yellow
Write-Host "   • Login: admin@example.com / admin123" -ForegroundColor White
Write-Host "   • 25 Ports across 6 continents" -ForegroundColor White
Write-Host "   • 60+ Ships from major shipping companies" -ForegroundColor White
Write-Host "   • 300 Containers with diverse cargo types" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop the application" -ForegroundColor Gray
Write-Host ""

dotnet run