using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateworkOrderRepairTasks;

public sealed record UpdateWorkOrderRepairTasksCommand(Guid WorkOrderId, Guid[] RepairTasks)
    : IRequest<Result<Updated>>;
