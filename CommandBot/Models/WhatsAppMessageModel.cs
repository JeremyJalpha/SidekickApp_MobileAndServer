using System.Text.Json.Serialization;

public class Status
{
    [JsonPropertyName("id")]
    public string? ID { get; set; }

    [JsonPropertyName("status")]
    public string? StatusText { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("recipient_id")]
    public string? RecipientID { get; set; }

    [JsonPropertyName("conversation")]
    public Conversation? Conversation { get; set; }

    [JsonPropertyName("pricing")]
    public Pricing? Pricing { get; set; }
}

public class Conversation
{
    [JsonPropertyName("id")]
    public string? ID { get; set; }

    [JsonPropertyName("expiration_timestamp")]
    public string? ExpirationTimestamp { get; set; }

    [JsonPropertyName("origin")]
    public Origin? Origin { get; set; }
}

public class Origin
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class Pricing
{
    [JsonPropertyName("billable")]
    public bool Billable { get; set; }

    [JsonPropertyName("pricing_model")]
    public string? PricingModel { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }
}

public class StatusesWebhookRequest
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("entry")]
    public List<Entry>? Entry { get; set; }
}

public class MessagesWebhookRequest
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("entry")]
    public List<Entry>? Entry { get; set; }
}

public class Entry
{
    [JsonPropertyName("id")]
    public string? ID { get; set; }

    [JsonPropertyName("changes")]
    public List<Change>? Changes { get; set; }
}

public class Change
{
    [JsonPropertyName("value")]
    public Value? Value { get; set; }

    [JsonPropertyName("field")]
    public string? Field { get; set; }
}

public class Value
{
    [JsonPropertyName("messaging_product")]
    public string? MessagingProduct { get; set; }

    [JsonPropertyName("metadata")]
    public Metadata? Metadata { get; set; }

    [JsonPropertyName("messages")]
    public List<Message>? Messages { get; set; }

    [JsonPropertyName("statuses")]
    public List<Status>? Statuses { get; set; }
}

public class Metadata
{
    [JsonPropertyName("display_phone_number")]
    public string? DisplayPhoneNumber { get; set; }

    [JsonPropertyName("phone_number_id")]
    public string? PhoneNumberID { get; set; }
}

public class Message
{
    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("id")]
    public string? ID { get; set; }

    [JsonPropertyName("timestamp")]
    public string? Timestamp { get; set; }

    [JsonPropertyName("text")]
    public TextContent? Text { get; set; }
}

public class TextContent
{
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}