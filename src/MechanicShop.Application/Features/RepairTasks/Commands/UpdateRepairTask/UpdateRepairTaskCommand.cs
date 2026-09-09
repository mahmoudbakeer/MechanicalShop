using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enum;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed record UpdateRepairTaskCommand(
    Guid Id,
    string Name,
    RepairTaskDuration Duration,
    decimal LaborCost,
    List<UpdatePartCommand> Parts
) : IRequest<Result<Updated>>;
