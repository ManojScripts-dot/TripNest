using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TripNest.Data;
using TripNest.Services;
using TripNest.Models;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file in development
if (builder.Environment.IsDevelopment())
{
    var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (File.Exists(envPath))
    {
        var envVars = File.ReadAllLines(envPath)
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            .Select(line => line.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1].Trim('"'));

        foreach (var envVar in envVars)
        {
            Environment.SetEnvironmentVariable(envVar.Key, envVar.Value);
        }
    }
}

// Get connection string from environment variables
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string not found. Please set ConnectionStrings__DefaultConnection environment variable.");
}

// Register DbContext with PostgreSQL provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Cloudinary service
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; 
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2); 
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; 
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrations applied successfully");
        
        // Seed sample data if database is empty
        if (!context.Tours.Any())
        {
            app.Logger.LogInformation("Seeding sample data...");
            
            // Create sample agency
            var sampleAgency = new Agency
            {
                Name = "Adventure Tours Nepal",
                Email = "info@adventuretours.com",
                PhoneNumber = "+977-1-1234567",
                Password = "sample_password" // In production, this should be hashed
            };
            context.Agencies.Add(sampleAgency);
            await context.SaveChangesAsync();
            
            // Create sample tours
            var sampleTours = new List<Tour>
            {
                new Tour
                {
                    Title = "Everest Base Camp Trek",
                    Description = "Experience the ultimate adventure with our 14-day trek to Everest Base Camp. Witness breathtaking mountain views, Sherpa culture, and the world's highest peak.",
                    DurationDays = 14,
                    StarRating = 5,
                    Price = 1200.00m,
                    ImagePath = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg",
                    Destination = "Everest Region, Nepal",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow,
                    AgencyId = sampleAgency.Id
                },
                new Tour
                {
                    Title = "Annapurna Circuit Trek",
                    Description = "Discover the diverse landscapes of the Annapurna region on this 12-day trek. From lush valleys to high mountain passes, experience Nepal's natural beauty.",
                    DurationDays = 12,
                    StarRating = 4,
                    Price = 950.00m,
                    ImagePath = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg",
                    Destination = "Annapurna Region, Nepal",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow,
                    AgencyId = sampleAgency.Id
                },
                new Tour
                {
                    Title = "Kathmandu Cultural Tour",
                    Description = "Explore the rich cultural heritage of Kathmandu Valley. Visit UNESCO World Heritage sites, ancient temples, and traditional markets.",
                    DurationDays = 3,
                    StarRating = 4,
                    Price = 250.00m,
                    ImagePath = "https://res.cloudinary.com/dtudiub1v/image/upload/v1752921619/default-tour_clzafv.jpg",
                    Destination = "Kathmandu, Nepal",
                    Status = "Active",
                    CreatedDate = DateTime.UtcNow,
                    AgencyId = sampleAgency.Id
                }
            };
            
            context.Tours.AddRange(sampleTours);
            await context.SaveChangesAsync();
            
            app.Logger.LogInformation("Sample data seeded successfully");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error applying database migrations: {Error}", ex.Message);
    }
}

// Log configuration on startup (without exposing sensitive data)
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application starting up...");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

var cloudName = Environment.GetEnvironmentVariable("Cloudinary__CloudName") 
               ?? builder.Configuration["Cloudinary:CloudName"];
logger.LogInformation("Cloudinary configured: {IsConfigured}", !string.IsNullOrEmpty(cloudName));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Only use HTTPS redirection in production with HTTPS
if (app.Environment.IsProduction() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HTTPS")))
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add health check endpoint for Render
app.MapGet("/health", () => "OK");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();