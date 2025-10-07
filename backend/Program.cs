using Backend.Data;
using Backend.Extensions;
using Backend.Services;
using Backend.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using System.Text.Json.Serialization;
using System.Text;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Container Tracking & Port Operations API",
        Version = "v1",
        Description = "API for managing shipping containers, ports, ships, and berths with role-based access control",
    });
    
    // Configure Swagger to use XML comments
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add controllers and API explorer with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle reference loops in entity relationships
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        
        // Use camel case for property names
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        
        // Don't include null values in the output
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add DbContext - Connect directly to ContainerTrackingDB (no postgres dependency)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Override with environment variables if they exist
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "ContainerTrackingDB";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("DB_PASSWORD environment variable is not set.");
var dbSslMode = Environment.GetEnvironmentVariable("DB_SSL_MODE") ?? "Prefer";

// Build connection string with secure SSL support for Azure (removed Trust Server Certificate=true)
connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode={dbSslMode}";

Console.WriteLine($"Connecting to database: {dbName} on {dbHost}:{dbPort} with SSL Mode={dbSslMode}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add JWT Configuration
var jwtKeyFromConfig = builder.Configuration["Jwt:Key"];
var jwtKeyFromEnv = Environment.GetEnvironmentVariable("JWT_KEY");

// Use environment variable if config key is null or empty
var jwtKey = string.IsNullOrEmpty(jwtKeyFromConfig) ? jwtKeyFromEnv : jwtKeyFromConfig;
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT_KEY is not configured in either appsettings.json or environment variables");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "ContainerTrackingAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "ContainerTrackingClients";

// Convert Base64 JWT key to bytes - SECURITY: Enforce valid Base64
byte[] jwtKeyBytes;
try
{
    jwtKeyBytes = Convert.FromBase64String(jwtKey);
}
catch (FormatException ex)
{
    // SECURITY FIX: Enforce that JWT key must be valid Base64 for production
    throw new InvalidOperationException(
        "JWT_KEY must be a valid Base64 string for security. " +
        "Generate one using: [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes('your-secret-key'))", 
        ex);
}

builder.Configuration["Jwt:Key"] = jwtKey;
builder.Configuration["Jwt:Issuer"] = jwtIssuer;
builder.Configuration["Jwt:Audience"] = jwtAudience;
builder.Configuration["Jwt:ExpirationMinutes"] = builder.Configuration["Jwt:ExpirationMinutes"] ?? "60";

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // SECURITY: Always require HTTPS in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Register repositories and services
builder.Services.RegisterRepositories()
                .RegisterServices();

// Register authentication and role management services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleApplicationService, RoleApplicationService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IDataSeedService, DataSeedService>();

// Add CORS policy with secure configuration for production
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")?.Split(',') 
    ?? new[] { "http://localhost:3000", "http://localhost:4200", "http://localhost:5173", "http://localhost:5174" };

var isProduction = builder.Environment.IsProduction();

builder.Services.AddCors(options =>
{
    if (isProduction)
    {
        // SECURITY: Strict CORS for production
        options.AddPolicy("ProductionCors", 
            policy => policy
                .WithOrigins(corsOrigins) // Only specified origins
                .WithMethods("GET", "POST", "PUT", "DELETE") // Only required methods
                .WithHeaders("Content-Type", "Authorization") // Only required headers
                .AllowCredentials()
                .SetPreflightMaxAge(TimeSpan.FromMinutes(10))); // Cache preflight
    }
    else
    {
        // Development CORS (more permissive)
        options.AddPolicy("DevelopmentCors", 
            policy => policy
                .WithOrigins(corsOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// Add global exception handler middleware
app.UseGlobalExceptionHandler();

// Add security headers for production
if (app.Environment.IsProduction())
{
    app.UseSecurityHeaders();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Container Tracking API v1");
        options.RoutePrefix = string.Empty;
    });
    
    // Use development-specific CORS policy
    app.UseCors("DevelopmentCors");
}
else
{
    // SECURITY: Use strict CORS for production
    app.UseCors("ProductionCors");
}

app.UseHttpsRedirection();

app.UseRouting();

// Authentication and Authorization middleware (order is important)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Use the new production seeder for clean, consistent data
        await Backend.Data.Seeding.ProductionDataSeeder.SeedAsync(context, logger);
        
        logger.LogInformation("Database initialized and seeded successfully with production data");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database");
        throw;
    }
}

app.Run();
