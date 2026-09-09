using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrder.Events;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;

public class UpdateWorkOrderStateCommandHandler(
    IAppDbContext context,
    ILogger<UpdateWorkOrderStateCommandHandler> logger,
    HybridCache cache
) : IRequestHandler<UpdateWorkOrderStateCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateWorkOrderStateCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(
        UpdateWorkOrderStateCommand request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context.WorkOrders.FindAsync(
            [request.WorkOrderId],
            cancellationToken
        );
        if (workOrder is null)
        {
            _logger.LogWarning(
                "Updating work order state failed, WorkOrder with ID {WorkOrderId} does not exist.",
                request.WorkOrderId
            );
            return ApplicationErrors.WorkOrderNotFound;
        }
        if (workOrder.StartedAtUtc > DateTimeOffset.UtcNow)
        {
            _logger.LogWarning(
                "Updating work order state failed, WorkOrder with ID {WorkOrderId} cannot be updated to state {NewState} because it has not started yet.",
                request.WorkOrderId,
                request.NewState
            );
            return WorkOrderError.StateTransformationNotAllowed(workOrder.StartedAtUtc);
        }
        var result = workOrder.UpdateState(request.NewState);
        if (result.IsError)
        {
            _logger.LogWarning(
                "Updating work order state failed, WorkOrder with ID {WorkOrderId} cannot be updated to state {NewState}.",
                request.WorkOrderId,
                request.NewState
            );
            return result.Errors!;
        }
        if (workOrder.State == WorkOrderState.Completed)
        {
            workOrder.AddDomainEvent(new WorkOrderCompleted { WorkOrderId = workOrder.Id });
        }
        workOrder.AddDomainEvent(new WorkOrderCollectionModified());
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("work-order", cancellationToken);
        return Result.Updated;
    }
}
