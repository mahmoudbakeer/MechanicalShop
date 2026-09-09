using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Application.Features.WorkOrders.WorkOrderMappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees.Enum;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandHandler(
    IAppDbContext context,
    ILogger<CreateWorkOrderCommandHandler> logger,
    HybridCache cache,
    IWorkOrderPolicy workOrderValidator,
    TimeProvider dateTimeProvider
) : IRequestHandler<CreateWorkOrderCommand, Result<WorkOrderDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateWorkOrderCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    private readonly IWorkOrderPolicy _workOrderValidator = workOrderValidator;
    private readonly TimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task<Result<WorkOrderDto>> Handle(
        CreateWorkOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var laborexist = await _context.Employees.AnyAsync(
            l => l.Id == request.LaborId && l.Role == Role.Labor,
            cancellationToken
        );

        if (!laborexist)
        {
            _logger.LogWarning(
                "Labor with ID {LaborId} does not exist or is not a labor.",
                request.LaborId
            );
            return ApplicationErrors.LaborNotFound;
        }

        var vehicleExists = await _context.Vehicles.AnyAsync(
            v => v.Id == request.VehicleId,
            cancellationToken
        );

        if (!vehicleExists)
        {
            _logger.LogWarning("Vehicle with ID {VehicleId} does not exist.", request.VehicleId);
            return ApplicationErrors.VehicleNotFound;
        }

        var repairTasks = await _context
            .RepairTasks.Where(rt => request.RepairTaskIds.Contains(rt.Id))
            .ToListAsync(cancellationToken);

        if (repairTasks.Count != request.RepairTaskIds.Count)
        {
            _logger.LogWarning(
                "One or more repair tasks do not exist. Requested: {RequestedRepairTaskIds}, Found: {FoundRepairTaskIds}",
                request.RepairTaskIds,
                repairTasks.Select(rt => rt.Id)
            );
            return ApplicationErrors.RepairTaskNotFound;
        }
        if (!Enum.TryParse<Spot>(request.Spot, true, out var spot))
        {
            _logger.LogWarning("Invalid spot value: {Spot}", request.Spot);
            return WorkOrderError.InvalidSpot;
        }

        var duration = TimeSpan.FromMinutes(repairTasks.Sum(rt => (int)rt.EstimatedDuration));
        var endAt = request.StartAt.Add(duration);
        var resultMinimumRequirement = _workOrderValidator.ValidateMinimumRequirement(
            request.StartAt,
            endAt
        );
        if (resultMinimumRequirement.IsError)
        {
            _logger.LogWarning(
                "Work order timing does not meet minimum requirements. Start: {StartAt}, End: {EndAt}",
                request.StartAt,
                endAt
            );
            return resultMinimumRequirement.Errors!;
        }
        if (_workOrderValidator.IsOutsideBusinessHours(request.StartAt, duration))
        {
            _logger.LogWarning(
                "Work order timing is outside business hours. Start: {StartAt}, End: {EndAt}",
                request.StartAt,
                endAt
            );
            return ApplicationErrors.WorkOrderTimeOutsideWorkingHours(request.StartAt, endAt);
        }

        if (
            await _workOrderValidator.IsSpotAvailableAsync(
                spot: spot,
                startTime: request.StartAt,
                endAt,
                ct: cancellationToken
            )
        )
        {
            _logger.LogWarning(
                "Spot {Spot} is already occupied during the requested time range. Start: {StartAt}, End: {EndAt}",
                request.Spot,
                request.StartAt,
                endAt
            );
            return ApplicationErrors.SpotNotAvailable(request.StartAt, endAt);
        }
        if (
            await _workOrderValidator.IsVehicleAlreadyScheduledAsync(
                request.VehicleId,
                request.StartAt,
                endAt,
                cancellationToken
            )
        )
        {
            _logger.LogWarning(
                "Vehicle {VehicleId} is already scheduled for another work order during the requested time range. Start: {StartAt}, End: {EndAt}",
                request.VehicleId,
                request.StartAt,
                endAt
            );
            return ApplicationErrors.VehicleSchedulingConflict;
        }

        if (
            await _workOrderValidator.IsLaborOccupiedAsync(
                request.LaborId,
                startTime: request.StartAt,
                endAt,
                ct: cancellationToken
            )
        )
        {
            _logger.LogWarning(
                "Labor {LaborId} is already occupied during the requested time range. Start: {StartAt}, End: {EndAt}",
                request.LaborId,
                request.StartAt,
                endAt
            );
            return ApplicationErrors.LaborOccupied;
        }

        var workOrderResult = WorkOrder.Create(
            id: Guid.NewGuid(),
            employeeId: request.LaborId,
            startAt: request.StartAt,
            endAt,
            vehicleId: request.VehicleId,
            spot: spot,
            repairTasks: repairTasks,
            Now: _dateTimeProvider.GetUtcNow()
        );
        if (workOrderResult.IsError)
        {
            _logger.LogWarning("Failed to create work order.");
            return workOrderResult.Errors!;
        }
        var workOrder = workOrderResult.Value;

        _context.WorkOrders.Add(workOrder);

        workOrder.AddDomainEvent(new WorkOrderCollectionModified());

        await _context.SaveChangesAsync(cancellationToken);
        await _cache.RemoveByTagAsync("work-order", cancellationToken);

        return workOrder.ToDto();
    }
}
