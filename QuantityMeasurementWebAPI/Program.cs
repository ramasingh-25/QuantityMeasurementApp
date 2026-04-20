using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using QuantityMeasurementBusinessLayer.Interfaces;
using QuantityMeasurementBusinessLayer.Services;
using QuantityMeasurementRepositoryLayer.Context;
using QuantityMeasurementRepositoryLayer.Interfaces;
using QuantityMeasurementRepositoryLayer.Repository;
using QuantityMeasurementRepositoryLayer.Repositories;
using QuantityMeasurementWebAPI.Middleware;
using QuantityMeasurementWebAPI.Services;
using System.Security.Claims;
using Npgsql;

// Helper method to convert Render's DATABASE_URL to Npgsql connection string
string ConvertRenderDatabaseUrlToNpgsql(string databaseUrl)
{
    Console.WriteLine($"Converting DATABASE_URL: {databaseUrl}");
    
    // Render provides DATABASE_URL in format: postgresql://user:password@host:port/database
    if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var username = userInfo[0];
        var password = userInfo.Length > 1 ? userInfo[1] : "";
        
        // Default to PostgreSQL port 5432 if not specified
        var port = uri.Port > 0 ? uri.Port : 5432;
        
        var connectionString = $"Host={uri.Host};Port={port};Database={uri.AbsolutePath.Trim('/')};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
        Console.WriteLine($"Converted connection string: {connectionString}");
        return connectionString;
    }
    
    Console.WriteLine("DATABASE_URL doesn't start with postgres:// or postgresql://, returning as-is");
    return databaseUrl; // Return as-is if not in expected format
}

var builder = WebApplication.CreateBuilder(args);

// ---------------------- Add Services ----------------------

// Controllers
builder.Services.AddControllers();

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        RoleClaimType = ClaimTypes.Role,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKey1234567890123456")
        )
    };

    // For debugging - log authentication events
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

// CORS - Allow frontend origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Allow all origins in development
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            // Allow all origins in production for Render deployment
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// Authorization
builder.Services.AddAuthorization();

// Swagger + JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuantityMeasurement API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter JWT token",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition("Bearer", jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

// ---------------------- Database ----------------------
builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Use SQL Server in development
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Use PostgreSQL in production (Render) - use DATABASE_URL
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        Console.WriteLine($"DATABASE_URL environment variable: {databaseUrl}");
        
        if (!string.IsNullOrEmpty(databaseUrl))
        {
            // Convert Render's DATABASE_URL format to Npgsql connection string
            var connectionString = ConvertRenderDatabaseUrlToNpgsql(databaseUrl);
            Console.WriteLine($"Using connection string: {connectionString}");
            options.UseNpgsql(connectionString);
        }
        else
        {
            // Fallback to DefaultConnection if DATABASE_URL is not set
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"DATABASE_URL not found, using DefaultConnection: {connectionString}");
            options.UseNpgsql(connectionString);
        }
    }
});

// ---------------------- Dependency Injection ----------------------
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<QuantityMeasurementEFRepository>();
// builder.Services.AddScoped<QuantityMeasurementCacheRepository>();

// Services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementServiceImpl>();
// builder.Services.AddHostedService<RedisSyncBackgroundService>();

// Redis cache - using in-memory fallback when Redis is not available
builder.Services.AddDistributedMemoryCache();
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = "localhost:6379";
//     options.InstanceName = "QuantityMeasurement_";
// });

// ---------------------- Build App ----------------------
var app = builder.Build();

// Auto-create/migrate database (creates QuantityMeasurements + Users tables)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuantityMeasurementDbContext>();
    
    if (builder.Environment.IsDevelopment())
    {
        // SQL Server specific migration handling
        db.Database.ExecuteSqlRaw("""
            IF OBJECT_ID('[__EFMigrationsHistory]') IS NULL
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL,
                    [ProductVersion] nvarchar(32) NOT NULL,
                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                );
            IF OBJECT_ID('[QuantityMeasurements]') IS NOT NULL
                AND NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260331094451_InitialCreate')
                INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                VALUES ('20260331094451_InitialCreate', '9.0.5');
            """);
    }
    
    db.Database.Migrate();
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuantityMeasurement API V1");
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();