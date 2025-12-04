using BookStore.Data;
using BookStore.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
    options.Password.RequireNonAlphanumeric = true; // special char
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplDbContext>()
.AddDefaultTokenProviders();


// Add HttpClient for API
builder.Services.AddHttpClient("BookStore", client =>
{
    client.BaseAddress = new Uri("https://localhost:44364/");
});

// Add distributed memory cache required by session
builder.Services.AddDistributedMemoryCache();

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

    // Allow root path (default route -> Account/Login) so form posting to "/" isn't blocked
    if (path == "/")
    {
        await next();
        return;
    }

    // Skip JWT check for login/register GET & POST (MVC)
    if (path.StartsWith("/account/login") || path.StartsWith("/account/register"))
    {
        await next();
        return;
    }

    // Skip static files
    if (path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/images"))
    {
        await next();
        return;
    }

    // Allow unauthenticated access to API login/register endpoints
    if (path.StartsWith("/api/auth/login") || path.StartsWith("/api/auth/register"))
    {
        await next();
        return;
    }

    // Check JWT token in session
    var token = context.Session.GetString("JWToken");
    if (string.IsNullOrEmpty(token))
    {
        if (path.StartsWith("/api"))
        {
            // Don't redirect API requests; return401 so client can handle it
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        Console.WriteLine("No token found - redirecting to /Account/Login");
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
