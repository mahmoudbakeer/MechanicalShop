using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrder.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.WorkOrderEventsHanlder;

public class WorkOrderCompletedEmailHandler(
    IAppDbContext context,
    INotificationService notificationService,
    ILogger<WorkOrderCompletedEmailHandler> logger
) : INotificationHandler<WorkOrderCompleted>
{
    private readonly IAppDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;
    private readonly ILogger<WorkOrderCompletedEmailHandler> _logger = logger;

    public async Task Handle(WorkOrderCompleted notification, CancellationToken cancellationToken)
    {
        var WorkOrder = await _context
            .WorkOrders.Include(wo => wo.Vehicle)
                .ThenInclude(v => v.Customer)
            .FirstOrDefaultAsync(wo => wo.Id == notification.WorkOrderId, cancellationToken);

        if (WorkOrder is null)
        {
            _logger.LogWarning(
                "WorkOrder with Id {WorkOrderId} not found.",
                notification.WorkOrderId
            );
            return;
        }
        await _notificationService.SendEmailAsync(
            WorkOrder.Vehicle?.Customer?.Email!,
            cancellationToken
        );
        await _notificationService.SendSmsAsync(
            WorkOrder.Vehicle?.Customer?.PhoneNumber!,
            cancellationToken
        );
        return;
    }
}
