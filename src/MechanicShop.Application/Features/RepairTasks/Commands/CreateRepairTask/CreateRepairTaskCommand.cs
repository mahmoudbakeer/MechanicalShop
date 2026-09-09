using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks.Enum;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed record CreateRepairTaskCommand(
    string Name,
    RepairTaskDuration Duration,
    decimal LaborCost,
    List<CreatePartCommand> Parts
) : IRequest<Result<RepairTaskDto>>;
