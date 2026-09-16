using MechanicShop.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Infrastructure.Services;

public sealed class NotificationService(ILogger<NotificationService> logger) : INotificationService
{
    private readonly ILogger<NotificationService> _logger = logger;

    private const string Message =
        "Your vehicle service is complete. You may collect it from the shop at your earliest convenience.";

    public Task SendEmailAsync(string Email, CancellationToken token)
    {
        var at = Email.IndexOf('@');
        var maskedEmail =
            at > 1 ? Email[0] + new string('*', at - 2) + Email[at - 1] + Email[at..] : "*****";

        _logger.LogInformation("[Email] To: {Email} | Message: {Message}", maskedEmail, Message);
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string PhoneNumber, CancellationToken token)
    {
        var maskedPhone =
            PhoneNumber.Length > 4
                ? new string('*', PhoneNumber.Length - 4) + PhoneNumber[4..]
                : "*****";
        _logger.LogInformation(
            "[SMS] To: {PhoneNumber} | Message: {Message}",
            maskedPhone,
            Message
        );
        return Task.CompletedTask;
    }
}
