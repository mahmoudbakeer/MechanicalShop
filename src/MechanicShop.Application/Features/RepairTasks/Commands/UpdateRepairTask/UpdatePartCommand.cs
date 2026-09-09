using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public sealed record class UpdatePartCommand(Guid Id, string Name, decimal Cost, int Quantity)
    : IRequest<Result<Updated>>;
