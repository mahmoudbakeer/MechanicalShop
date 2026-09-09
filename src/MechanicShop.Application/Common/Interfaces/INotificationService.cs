namespace MechanicShop.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendEmailAsync(string Email, CancellationToken token);
    Task SendSmsAsync(string PhoneNumber, CancellationToken token);
}
