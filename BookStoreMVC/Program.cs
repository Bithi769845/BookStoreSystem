using BookStore.Data;
using BookStore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookStoreSystem.Services; // IPermissionService / PermissionService

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplDbContext>()
.AddDefaultTokenProviders();

// Register permission service for ModuleAuthorizeAttribute
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Add HttpClient for API
builder.Services.AddHttpClient("BookStore", client =>
{
    client.BaseAddress = new Uri("https://localhost:44364/");
});

// Add distributed memory cache
builder.Services.AddDistributedMemoryCache();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure access denied path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; // make sure this page exists
});

var app = builder.Build();

// Seed Admin user (before app.Run)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    await SeedAdmin(userManager, roleManager);
}

// Exception & HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

// JWT Middleware
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    if (path == "/") { await next(); return; }
    if (path.StartsWith("/account/login") || path.StartsWith("/account/register")) { await next(); return; }
    if (path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images")) { await next(); return; }
    if (path.StartsWith("/api/auth/login") || path.StartsWith("/api/auth/register")) { await next(); return; }

    var token = context.Session.GetString("JWToken");
    if (string.IsNullOrEmpty(token))
    {
        if (path.StartsWith("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

// --------------------------------------------------------
// Admin seeding method
// --------------------------------------------------------
async Task SeedAdmin(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
{
    // Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
    }

    // Admin user info
    var adminEmail = "admin@local";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // Create new admin user
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Administrator"
        };
        await userManager.CreateAsync(adminUser, "Admin123!"); // set default password
        await userManager.AddToRoleAsync(adminUser, "Admin");
        Console.WriteLine("Admin user created: admin@local / Admin123!");
    }
    else
    {
        // Reset password if user exists
        var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
        await userManager.ResetPasswordAsync(adminUser, token, "Admin123!");
        Console.WriteLine("Admin password reset to: Admin123!");
    }
}
