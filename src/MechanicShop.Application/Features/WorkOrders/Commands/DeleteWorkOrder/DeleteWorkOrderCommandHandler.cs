using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;

public class DeleteWorkOrderCommandHandler(
    IAppDbContext context,
    ILogger<DeleteWorkOrderCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<DeleteWorkOrderCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<DeleteWorkOrderCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Deleted>> Handle(
        DeleteWorkOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context.WorkOrders.FindAsync(
            [request.WorkOrderId],
            cancellationToken
        );
        if (workOrder is null)
        {
            _logger.LogWarning("Work order with ID {WorkOrderId} not found.", request.WorkOrderId);
            return ApplicationErrors.WorkOrderNotFound;
        }

        if (workOrder.State != WorkOrderState.Scheduled)
        {
            _logger.LogWarning(
                "Work order with ID {WorkOrderId} is not in a deletable state.",
                request.WorkOrderId
            );
            return WorkOrderError.ReadOnly;
        }

        _context.WorkOrders.Remove(workOrder);

        workOrder.AddDomainEvent(new WorkOrderCollectionModified());
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("work-order", cancellationToken);

        return Result.Deleted;
    }
}
