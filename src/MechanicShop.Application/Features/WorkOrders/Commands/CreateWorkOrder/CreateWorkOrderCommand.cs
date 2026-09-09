using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public sealed record CreateWorkOrderCommand(
    Guid LaborId,
    Guid VehicleId,
    DateTimeOffset StartAt,
    string Spot,
    List<Guid> RepairTaskIds
) : IRequest<Result<WorkOrderDto>>;
