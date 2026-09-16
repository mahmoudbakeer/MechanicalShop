using MechanicShop.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MechanicShop.Infrastructure.RealTime;

public class SignalRWorkOrderNotifier(IHubContext<WorkOrderHub> hubContext) : IWorkOrderNotifier
{
    public async Task NotifyWorkOrderChangedAsync(CancellationToken cancellationToken)
    {
        await hubContext.Clients.All.SendAsync("WorkOrderChanged", cancellationToken);
    }
}
