using System.Text;
using BloodConnect.Core.Interfaces;
using BloodConnect.Infrastructure.Data;
using BloodConnect.Infrastructure.Repositories;
using BloodConnect.Infrastructure.Seeds;
using BloodConnect.Services.Services;
using BloodConnectApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(origin => true)  // Allows any origin dynamically
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Required for SignalR WebSockets
    });
});

// Configure Database
builder.Services.AddDbContext<BloodConnectDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Register repositories and services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IScreeningService, ScreeningService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IDeferralReasonService, DeferralReasonService>();
builder.Services.AddScoped<IPrinterService, PrinterService>();

var app = builder.Build();

// Initialize database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BloodConnectDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            logger.LogInformation("Found {PendingMigrationCount} pending migration(s). " +
                                  "Applying migrations...", pendingMigrations.Count());

            // Apply pending migrations asynchronously
            await dbContext.Database.MigrateAsync();
            
            logger.LogInformation("\n" +
              "╔══════════════════════════════════════════════════════╗\n" +
              "║            DATABASE MIGRATION COMPLETE               ║\n" +
              "╚══════════════════════════════════════════════════════╝\n");
        }
        else
        {
            logger.LogInformation("\n" +
              "╔══════════════════════════════════════════════════════╗\n" +
              "║            NO PENDING MIGRATIONS FOUND               ║\n" +
              "╚══════════════════════════════════════════════════════╝\n");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "\n" +
            "╔══════════════════════════════════════════════════════════════════╗\n" +
            "║  ❌ DATABASE CONNECTION FAILED                                   ║\n" +
            "╠══════════════════════════════════════════════════════════════════╣\n" +
            "║  The application cannot start without a database connection.     ║\n" +
            "║                                                                  ║\n" +
            "║  To start the database:                                          ║\n" +
            "║    make db-up                                                    ║\n" +
            "║                                                                  ║\n" +
            "║  Or configure a connection string in appsettings.Development.json║\n" +
            "╚══════════════════════════════════════════════════════════════════╝");
        
        // Always fail startup if database is not available
        throw new InvalidOperationException("Database connection required. See logs for details.", ex);
    }
}
// Seed database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BloodConnectDbContext>();
    await DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blood Connect API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the root
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
