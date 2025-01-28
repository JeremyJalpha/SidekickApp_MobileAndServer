using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SidekickApp_WebAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key"))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure CORS for Development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("CorsPolicy");
}
else
{
    // For production, consider a more security-focused approach:
    app.UseCors(builder =>
    {
        builder.WithOrigins("https://trusted-production-domain.com") // Replace with trusted domain(s)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Use top-level routing.
app.MapControllers();
app.MapHub<AuthenticationHub>("/AuthenticationHub");

app.Run();
