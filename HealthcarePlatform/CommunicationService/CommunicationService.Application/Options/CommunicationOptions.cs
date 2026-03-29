namespace CommunicationService.Application.Options;

public sealed class CommunicationOptions
{
    public const string SectionName = "Communication";

    public ReferenceValueIdsOptions ReferenceValueIds { get; set; } = new();

    public NotificationProcessorOptions Processor { get; set; } = new();

    public SmtpOptions Smtp { get; set; } = new();

    public SmsApiOptions Sms { get; set; } = new();

    public WhatsAppApiOptions WhatsApp { get; set; } = new();
}

public sealed class ReferenceValueIdsOptions
{
    public long NotificationStatusDraft { get; set; }
    public long NotificationStatusQueued { get; set; }
    public long NotificationStatusCompleted { get; set; }
    public long NotificationStatusFailed { get; set; }

    public long ChannelDeliveryPending { get; set; }
    public long ChannelDeliverySent { get; set; }
    public long ChannelDeliveryFailed { get; set; }

    public long QueuePending { get; set; }
    public long QueueProcessing { get; set; }
    public long QueueCompleted { get; set; }
    public long QueueFailed { get; set; }

    public long LogAttemptSuccess { get; set; }
    public long LogAttemptFailed { get; set; }

    public long ChannelTypeEmail { get; set; }
    public long ChannelTypeSms { get; set; }
    public long ChannelTypeWhatsApp { get; set; }
}

public sealed class NotificationProcessorOptions
{
    public int PollIntervalSeconds { get; set; } = 5;

    public int BatchSize { get; set; } = 20;

    public int MaxRetryAttempts { get; set; } = 3;
}

public sealed class SmtpOptions
{
    public string Host { get; set; } = "localhost";

    public int Port { get; set; } = 25;

    public bool UseSsl { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string FromAddress { get; set; } = "noreply@localhost";

    public string FromName { get; set; } = "TriVita";
}

public sealed class SmsApiOptions
{
    public string BaseUrl { get; set; } = "https://sms.example.invalid/";

    public string ApiKey { get; set; } = string.Empty;

    public string SenderId { get; set; } = string.Empty;
}

public sealed class WhatsAppApiOptions
{
    public string BaseUrl { get; set; } = "https://whatsapp.example.invalid/";

    public string ApiKey { get; set; } = string.Empty;

    public string FromNumber { get; set; } = string.Empty;
}
