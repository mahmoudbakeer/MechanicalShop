using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public sealed record RelocateWorkOrderCommand(Guid WorkOrderId, DateTimeOffset StartAt, Spot Spot)
    : IRequest<Result<Updated>>;
