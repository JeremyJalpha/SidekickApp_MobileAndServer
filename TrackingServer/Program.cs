using MAS_Shared.Data;
using MAS_Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TrackingServer.Interfaces;
using TrackingServer.Services;
using TrackingServer.SignalR;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Load unified JWT config
builder.Services.Configure<JwtIssueConfig>(
    configuration.GetSection("Jwt"));

var jwtConfig = configuration.GetSection("Jwt").Get<JwtIssueConfig>();

if (string.IsNullOrWhiteSpace(jwtConfig?.Key))
    throw new InvalidOperationException("Invalid secret key");

if (jwtConfig?.Audiences == null || !jwtConfig.Audiences.Any())
    throw new InvalidOperationException("Invalid audiences");

if (string.IsNullOrWhiteSpace(jwtConfig.Issuer))
    throw new InvalidOperationException("Invalid issuer");

builder.Services.AddCors();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MAS_Shared")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // <- ?? this is important
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig.Issuer,
        ValidAudiences = jwtConfig.Audiences,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"? JWT Authentication failed: {context.Exception?.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("? JWT validated. Claims:");
            if (context.Principal != null)
            {
                foreach (var claim in context.Principal.Claims)
                    Console.WriteLine($"  - {claim.Type} = {claim.Value}");
            }
            else
            {
                Console.WriteLine("  - No principal found.");
            }
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DriverPolicy", policy => policy.RequireRole("Driver"));
});
builder.Services.AddSignalR();

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<ICoordinateService, CoordinateService>();
builder.Services.AddHostedService<LocationPollingService>();

Console.WriteLine($"?? JWT Issuer: {jwtConfig.Issuer}");
Console.WriteLine($"?? Audiences: {string.Join(", ", jwtConfig.Audiences)}");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseCors(policy => policy
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
}
else
{
    app.UseCors(policy =>
    {
        policy.WithOrigins("https://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapHub<AuthenticationHub>("/AuthenticationHub");
app.MapHub<GPSLocationHub>("/GPSLocationHub");

app.Run();