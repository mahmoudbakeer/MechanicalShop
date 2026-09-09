namespace MechanicShop.Application.Common.Interfaces;

public interface IWorkOrderNotifier
{
    Task NotifyWorkOrderChangedAsync(CancellationToken cancellationToken);
}
