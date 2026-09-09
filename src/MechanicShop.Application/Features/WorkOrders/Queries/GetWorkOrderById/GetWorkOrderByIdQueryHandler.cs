using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Application.Features.WorkOrders.WorkOrderMappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdQueryHandler(
    IAppDbContext context,
    ILogger<GetWorkOrderByIdQueryHandler> logger
) : IRequestHandler<GetWorkOrderByIdQuery, Result<WorkOrderDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetWorkOrderByIdQueryHandler> _logger = logger;

    public async Task<Result<WorkOrderDto>> Handle(
        GetWorkOrderByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context
            .WorkOrders.AsNoTracking()
            .Include(x => x.RepairTasks)
                .ThenInclude(rt => rt.Parts)
            .Include(x => x.Vehicle)
                .ThenInclude(v => v.Customer)
            .Include(x => x.Employee)
            .Include(x => x.Invoice)
            .FirstOrDefaultAsync(wo => wo.Id == request.WorkOrderId, cancellationToken);
        if (workOrder is null)
        {
            _logger.LogWarning("Work order with ID {WorkOrderId} not found.", request.WorkOrderId);
            return ApplicationErrors.WorkOrderNotFound;
        }
        return workOrder.ToDto();
    }
}
