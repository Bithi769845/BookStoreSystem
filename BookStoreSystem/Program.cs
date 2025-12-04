using BookStore.Data;
using BookStore.Helpers;
using BookStore.Models.Identity;
using BookStoreSystem.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add DbContext
builder.Services.AddDbContext<ApplDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add Identity
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

//JWT Authentication
//Reads configuration values from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//Enables authentication middleware
builder.Services.AddAuthentication(Options =>
{

    Options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    Options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(Options =>
    {
        Options.RequireHttpsMetadata = false; // When True,Token are only accepted over HTTPS
        Options.SaveToken = true;
        Options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))

        };
    });

builder.Services.AddAuthentication();

//Add JwtTokenGenerator
builder.Services.AddScoped<JwtTokenGenerator>();
builder.Services.AddScoped<IPermissionService, PermissionService>();


//swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookStore API", Version = "v1" });

    // Add JWT Bearer security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token"
    });

    // Make all endpoints require Bearer token by default
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

// Seed default admin user & role (development only)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminRole = "Admin";
        const string adminEmail = "admin@local";
        const string adminPassword = "Admin@123!";

        // create Admin role if not exists
        var roleExists = roleManager.RoleExistsAsync(adminRole).GetAwaiter().GetResult();
        if (!roleExists)
        {
            roleManager.CreateAsync(new ApplicationRole { Name = adminRole }).GetAwaiter().GetResult();
        }

        // create admin user if not exists
        var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator"
            };
            var createRes = userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();
            if (createRes.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, adminRole).GetAwaiter().GetResult();
                Console.WriteLine($"Seeded admin user: {adminEmail} / {adminPassword}");
            }
            else
            {
                Console.WriteLine("Failed to create seed admin: " + string.Join(';', createRes.Errors.Select(e => e.Description)));
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error during seeding: " + ex);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
