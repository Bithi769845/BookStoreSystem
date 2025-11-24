var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add HttpClient
builder.Services.AddHttpClient(
    "BookStore", client => {
        client.BaseAddress = new Uri("https://localhost:44364/");

    });

//Configuration for account
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Allow Login/Register without token
    if (path.Contains("/account/login") || path.Contains("/account/register"))
    {
        await next();
        return;
    }

    // Require JWT for all other pages
    var token = context.Session.GetString("JWToken");
    if (string.IsNullOrEmpty(token))
    {
        context.Response.Redirect("/Account/Login");
        return;
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
