using CommandBot.Services;
using MAS_Shared.Data;
using MAS_Shared.MASConstants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("webhook/whatsapp")]
public class WhatsAppController : ControllerBase
{
    private readonly ILogger<WhatsAppController> _logger;
    private readonly RabbitMQService _rabbitMQService;
    private readonly string _appSecret;

    public WhatsAppController(ILogger<WhatsAppController> logger, RabbitMQService rabbitMQService, IConfiguration configuration)
    {
        _logger = logger;
        _rabbitMQService = rabbitMQService;
        _appSecret = configuration["AppSecret"] ?? throw new InvalidOperationException("AppSecret is not set in the configuration.");
    }

    [HttpGet("")]
    public IActionResult RootEndpoint()
    {
        return Ok("Webhook service secured with SSL!");
    }

    [HttpGet]
    public IActionResult VerifyWebhook([FromQuery] string hub_verify_token, [FromQuery] string hub_challenge)
    {
        if (hub_verify_token == _appSecret)
        {
            _logger.LogInformation("Webhook verified.");
            return Ok(hub_challenge);
        }
        _logger.LogWarning("Error, wrong validation token.");
        return Forbid();
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook([FromBody] MessagesWebhookRequest request, [FromHeader(Name = "X-Hub-Signature-256")] string signature)
    {
        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Error, signature is missing.");
            return Forbid();
        }

        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        var computedSignature = CalculateSignatureSha256(payload, Encoding.UTF8.GetBytes(_appSecret));
        if (computedSignature != signature)
        {
            _logger.LogWarning("Error, signatures do not match.");
            return Forbid();
        }

        // Return HTTP 200 OK immediately before further processing
        _ = Task.Run(() => ProcessWebhook(request));

        return Ok("Success");
    }

    private Task ProcessWebhook(MessagesWebhookRequest request)
    {
        var messages = request.Entry?.FirstOrDefault()?.Changes?.FirstOrDefault()?.Value?.Messages;

        if (request == null || messages == null || messages.Count == 0)
        {
            _logger.LogWarning("Received webhook request with no messages.");
            return Task.CompletedTask;
        }

        if (messages.Count > 1)
        {
            _logger.LogWarning("Received webhook request with multiple messages, processing the first one only.");
        }

        string? msgOriginNumber = messages.FirstOrDefault()?.From;
        string? messageBody = messages.FirstOrDefault()?.Text?.Body;

        if (string.IsNullOrEmpty(msgOriginNumber) || string.IsNullOrEmpty(messageBody))
        {
            _logger.LogWarning("Received webhook request with missing data.");
            return Task.CompletedTask;
        }

        var messagePayload = new ChatUpdate
        {
            From = new ApplicationUser { CellNumber = msgOriginNumber },
            Body = messageBody,
            Channel = ChatChannelType.WhatsApp,
        };
        string serializedMessage = JsonSerializer.Serialize(messagePayload);
        _rabbitMQService.PublishCommand(serializedMessage);

        return Task.CompletedTask;
    }

    private static string CalculateSignatureSha256(string payload, byte[] secret)
    {
        using var mac = new HMACSHA256(secret);
        var rawHmac = mac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(rawHmac).Replace("-", "").ToLower();
    }
}