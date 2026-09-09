using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateworkOrderRepairTasks;

public class UpdateWorkOrderRepairTasksCommandHandler(
    IAppDbContext context,
    ILogger<UpdateWorkOrderRepairTasksCommandHandler> logger,
    IWorkOrderPolicy workOrderValidator,
    HybridCache cache
) : IRequestHandler<UpdateWorkOrderRepairTasksCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateWorkOrderRepairTasksCommandHandler> _logger = logger;
    private readonly IWorkOrderPolicy _workOrderValidator = workOrderValidator;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Updated>> Handle(
        UpdateWorkOrderRepairTasksCommand request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context
            .WorkOrders.Include(wo => wo.RepairTasks)
            .Include(wo => wo.Vehicle)
            .FirstOrDefaultAsync(wo => wo.Id == request.WorkOrderId, cancellationToken);
        if (workOrder is null)
        {
            _logger.LogWarning(
                "Updating work order repair tasks failed, WorkOrder with ID {WorkOrderId} does not exist.",
                request.WorkOrderId
            );
            return ApplicationErrors.WorkOrderNotFound;
        }
        if (workOrder.State == WorkOrderState.Completed)
        {
            _logger.LogWarning(
                "Updating work order repair tasks failed, WorkOrder with ID {WorkOrderId} cannot be updated to repair tasks {NewRepairTasks} because it is completed.",
                request.WorkOrderId,
                request.RepairTasks
            );
            return WorkOrderError.ReadOnly;
        }
        var tasks = await _context
            .RepairTasks.Where(rt => request.RepairTasks.Contains(rt.Id))
            .ToListAsync(cancellationToken);

        if (tasks.Count != request.RepairTasks.Count())
        {
            return ApplicationErrors.RepairTaskNotFound;
        }

        foreach (var t in tasks)
        {
            var repairTaskResult = workOrder.AddRepairTask(t);
            if (repairTaskResult.IsError)
            {
                _logger.LogWarning(
                    "Updating work order repair tasks failed, for RepairTask with ID {RepairTaskId}.",
                    t.Id
                );
                return repairTaskResult.Errors!;
            }
        }

        var totalDuration = TimeSpan.FromMinutes(
            workOrder.RepairTasks.Sum(rt => (int)rt.EstimatedDuration)
        );
        var newEndAt = workOrder.StartedAtUtc + totalDuration;

        if (_workOrderValidator.IsOutsideBusinessHours(workOrder.StartedAtUtc, totalDuration))
        {
            _logger.LogError(
                "Updating work order repair tasks failed, for WorkOrder with ID {WorkOrderId} because the new end time {NewEndAt} is outside business hours.",
                workOrder.Id,
                newEndAt
            );
            return ApplicationErrors.WorkOrderTimeOutsideWorkingHours(
                workOrder.StartedAtUtc,
                newEndAt
            );
        }
        if (
            await _workOrderValidator.IsVehicleAlreadyScheduledAsync(
                workOrder.VehicleId,
                workOrder.StartedAtUtc,
                newEndAt,
                cancellationToken,
                workOrder.Id
            )
        )
        {
            _logger.LogError(
                "Updating work order repair tasks failed, for WorkOrder with ID {WorkOrderId} because the vehicle is already scheduled for another work order during the new time range.",
                workOrder.Id
            );
            return ApplicationErrors.VehicleSchedulingConflict;
        }
        if (
            await _workOrderValidator.IsLaborOccupiedAsync(
                workOrder.EmployeeId,
                workOrder.StartedAtUtc,
                newEndAt,
                cancellationToken
            )
        )
        {
            _logger.LogError(
                "Updating work order repair tasks failed, for WorkOrder with ID {WorkOrderId} because the labor is occupied during the new time range.",
                workOrder.Id
            );
            return ApplicationErrors.LaborOccupied;
        }
        workOrder.AddDomainEvent(new WorkOrderCollectionModified());
        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync("work-order", cancellationToken);
        return Result.Updated;
    }
}
