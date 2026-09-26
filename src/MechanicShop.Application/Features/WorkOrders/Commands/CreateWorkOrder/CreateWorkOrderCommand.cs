using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public sealed record CreateWorkOrderCommand(
    Guid LaborId,
    Guid VehicleId,
    DateTimeOffset StartAt,
    Spot Spot,
    List<Guid> RepairTaskIds
) : IRequest<Result<WorkOrderDto>>;
