using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public class RelocateWorkOrderCommandHandler(
    IAppDbContext context,
    ILogger<RelocateWorkOrderCommandHandler> logger,
    IWorkOrderPolicy workOrderValidator,
    HybridCache cache
) : IRequestHandler<RelocateWorkOrderCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<RelocateWorkOrderCommandHandler> _logger = logger;
    private readonly IWorkOrderPolicy _workOrderValidator = workOrderValidator;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(
        RelocateWorkOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context
            .WorkOrders.Include(x => x.Employee)
            .Include(x => x.Vehicle)
            .Include(x => x.RepairTasks)
            .FirstOrDefaultAsync(r => r.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null)
        {
            _logger.LogWarning(
                "Relocating work order failed, WorkOrder with ID {WorkOrderId} does not exist.",
                request.WorkOrderId
            );
            return ApplicationErrors.WorkOrderNotFound;
        }

        var duration = workOrder.EndAtUtc.Subtract(workOrder.StartedAtUtc).Duration();
        var newEndAt = request.StartAt.Add(duration);
        if (_workOrderValidator.IsOutsideBusinessHours(request.StartAt, duration))
        {
            _logger.LogError(
                "Relocating work order failed, WorkOrder with ID {WorkOrderId} cannot be relocated to the new time range because it is outside of business hours.",
                workOrder.Id
            );
            return ApplicationErrors.WorkOrderTimeOutsideWorkingHours(request.StartAt, newEndAt);
        }
        if (
            !await _workOrderValidator.IsSpotAvailableAsync(
                request.Spot,
                request.StartAt,
                newEndAt,
                cancellationToken,
                workOrder.Id
            )
        )
        {
            _logger.LogError(
                "Relocating work order failed, WorkOrder with ID {WorkOrderId} cannot be relocated to the new spot because it is already occupied during the new time range.",
                workOrder.Id
            );
            return ApplicationErrors.SpotNotAvailable(request.StartAt, newEndAt);
        }

        if (
            await _workOrderValidator.IsVehicleAlreadyScheduledAsync(
                workOrder.VehicleId,
                request.StartAt,
                newEndAt,
                cancellationToken,
                workOrder.Id
            )
        )
        {
            _logger.LogError(
                "Relocating work order failed, for WorkOrder with ID {WorkOrderId} because the vehicle is already scheduled for another work order during the new time range.",
                workOrder.Id
            );
            return ApplicationErrors.VehicleSchedulingConflict;
        }
        if (
            await _workOrderValidator.IsLaborOccupiedAsync(
                workOrder.EmployeeId,
                request.StartAt,
                newEndAt,
                cancellationToken
            )
        )
        {
            _logger.LogError(
                "Relocating work order failed, for WorkOrder with ID {WorkOrderId} because the labor is occupied during the new time range.",
                workOrder.Id
            );
            return ApplicationErrors.LaborOccupied;
        }
        var updateTimeResult = workOrder.UpdateTiming(request.StartAt, newEndAt);
        if (updateTimeResult.IsError)
        {
            _logger.LogError(
                "Relocating work order failed, for WorkOrder with ID {WorkOrderId} because the new timing is invalid.",
                workOrder.Id
            );
            return updateTimeResult.Errors!;
        }

        var updateSpotResult = workOrder.UpdateSpot(request.Spot);

        if (updateSpotResult.IsError)
        {
            _logger.LogError(
                "Relocating work order failed, for WorkOrder with ID {WorkOrderId} because the new spot is invalid.",
                workOrder.Id
            );
            return updateSpotResult.Errors!;
        }

        workOrder.AddDomainEvent(new WorkOrderCollectionModified());

        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("work-order", cancellationToken);
        return Result.Updated;
    }
}
