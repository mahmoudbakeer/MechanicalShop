using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;

public class AssignLaborCommandHandler(
    IAppDbContext context,
    ILogger<AssignLaborCommandHandler> logger,
    IWorkOrderPolicy workOrderValidator,
    HybridCache cache
) : IRequestHandler<AssignLaborCommand, Result<Success>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<AssignLaborCommandHandler> _logger = logger;
    private readonly IWorkOrderPolicy _workOrderValidator = workOrderValidator;
    private readonly HybridCache _cache = cache;

    public async Task<Result<Success>> Handle(
        AssignLaborCommand request,
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
                "Assigning labor to workOrder failed, WorkOrder with ID {WorkOrderId} does not exist.",
                request.WorkOrderId
            );
            return ApplicationErrors.WorkOrderNotFound;
        }
        var labor = await _context.Employees.FirstOrDefaultAsync(
            e => e.Id == request.LaborId && e.Role == Role.Labor,
            cancellationToken
        );
        if (labor is null)
        {
            _logger.LogWarning(
                "Assigning labor to workOrder failed, Labor with ID {LaborId} does not exist.",
                request.LaborId
            );
            return ApplicationErrors.LaborNotFound;
        }
        if (
            await _workOrderValidator.IsLaborOccupiedAsync(
                request.LaborId,
                workOrder.StartedAtUtc,
                workOrder.EndAtUtc,
                cancellationToken
            )
        )
        {
            _logger.LogWarning(
                "Assigning labor to workOrder failed, Labor with ID {LaborId} is currently occupied and cannot be assigned to WorkOrder with ID {WorkOrderId}.",
                request.LaborId,
                request.WorkOrderId
            );
            return ApplicationErrors.LaborOccupied;
        }
        var result = workOrder.UpdateLabor(request.LaborId);
        if (result.IsError)
        {
            _logger.LogWarning(
                "Assigning labor to workOrder failed, Labor with ID {LaborId} cannot be assigned to WorkOrder with ID {WorkOrderId}.",
                request.LaborId,
                request.WorkOrderId
            );
            return result.Errors!;
        }
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveByTagAsync("work-order", cancellationToken);
        return Result.Success;
    }
}
