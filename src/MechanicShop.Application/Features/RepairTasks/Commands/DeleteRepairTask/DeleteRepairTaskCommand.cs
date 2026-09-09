using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Commands.DeleteRepairTask;

public sealed record DeleteRepairTaskCommand(Guid Id) : IRequest<Result<Deleted>>;
