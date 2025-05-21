using Microsoft.EntityFrameworkCore;
using TripNest.Data;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext with SQL Server connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add session support with a 30-minute timeout
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;   // Helps prevent client-side scripts from accessing the cookie
    options.Cookie.IsEssential = true; // Required if GDPR or similar cookie policies are applied
});

// Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();  // Make sure Session is enabled BEFORE Authorization

// If you add authentication middleware later, uncomment the following line
// app.UseAuthentication();

app.UseAuthorization();

// Default route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
