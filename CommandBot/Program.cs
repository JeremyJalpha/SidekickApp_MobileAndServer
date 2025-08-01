using CommandBot.Interfaces;
using CommandBot.Models;
using CommandBot.Services;
using CommandBot.Workers;
using MAS_Shared.Data;
using MAS_Shared.Interfaces.CommandBot.Interfaces;
using MAS_Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Bind and validate HostBusiness options
builder.Services.AddOptions<HostBusiness>()
    .Bind(configuration.GetSection("HostBusiness"))
    .Validate(o =>
        int.TryParse(o.Cellnumber, out _) &&
        Uri.TryCreate(o.BaseUrl, UriKind.Absolute, out _),
        "HostBusiness: Cellnumber must be numeric and BaseUrl must be a valid absolute URL.");

// Bind and validate JWT options
builder.Services.Configure<JwtIssueConfig>(configuration.GetSection("Jwt"));
builder.Services.AddOptions<JwtIssueConfig>()
    .Bind(configuration.GetSection("Jwt"))
    .Validate(opt =>
        !string.IsNullOrWhiteSpace(opt.Key) &&
        !string.IsNullOrWhiteSpace(opt.Issuer) &&
        opt.Audiences?.Count > 0,
        "Invalid JWT config");

// Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("MAS_Shared")
    ));

// Register services & command pipeline
builder.Services.AddScoped<ConversationContextFactory>();
builder.Services.AddScoped<BusinessContextFactory>();
builder.Services.AddScoped<ICommandRunner, CommandRunner>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddHostedService<CommandWorker>();
// Telegram-specific
builder.Services.AddSingleton<ITelegramDispatchService, TelegramDispatchService>();
builder.Services.AddHostedService<TelegramConsumerService>();

// WhatsApp-specific
builder.Services.AddSingleton<IWhatsAppDispatchService, WhatsAppDispatchService>();
builder.Services.AddHostedService<WhatsAppConsumerService>();

// BusinessContext is scoped (due to DbContext dependency)
builder.Services.AddScoped<BusinessContext>(sp =>
{
    var db = sp.GetRequiredService<AppDbContext>();
    var opts = sp.GetRequiredService<IOptions<HostBusiness>>().Value;
    var factory = sp.GetRequiredService<BusinessContextFactory>();
    return factory.CreateBusinessContext(
        db, 
        opts.BaseUrl, 
        opts.Cellnumber, 
        MAS_Shared.MASConstants.ChatChannelType.WhatsApp);
});

// CommandContext is per message
builder.Services.AddScoped<CommandContext>(sp =>
{
    var db = sp.GetRequiredService<AppDbContext>();
    var convo = sp.GetRequiredService<ConversationContext>();
    var busi = sp.GetRequiredService<BusinessContext>();
    var jwtConfig = sp.GetRequiredService<IOptions<JwtIssueConfig>>().Value;
    var logger = sp.GetRequiredService<ILogger<CommandContext>>();

    return new CommandContext(db, convo, busi, jwtConfig, logger);
});

var whatsappConfig = configuration.GetSection("WhatsApp").Get<WhatsAppBusinessCloudApiConfig>();
if (whatsappConfig == null)
{
    throw new InvalidOperationException("WhatsApp configuration is missing or invalid.");
}
builder.Services.AddWhatsAppBusinessCloudApiService(whatsappConfig);

// Configure Kestrel with HTTPS certificate
builder.WebHost.UseKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new X509Certificate2(
            "My_Keys/formatted_crt.pem",
            "DXgj43VikKvImniIds0p2d4q/O3Nruk5O/7MY/LN5Q4=");
    });
});

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Optional: IConfiguration registration
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var botToken = config.GetSection("Telegram")["BotToken"];
    if (string.IsNullOrEmpty(botToken))
    {
        throw new InvalidOperationException("TelegramBotToken is not set in the configuration.");
    }
    return new TelegramBotClient(botToken);
});

var app = builder.Build();

// Pipeline setup
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();