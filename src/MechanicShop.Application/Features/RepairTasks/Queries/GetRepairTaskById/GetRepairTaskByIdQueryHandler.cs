using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

public sealed class GetRepairTaskByIdQueryHandler(
    IAppDbContext context,
    ILogger<GetRepairTaskByIdQueryHandler> logger
) : IRequestHandler<GetRepairTaskByIdQuery, Result<RepairTaskDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetRepairTaskByIdQueryHandler> _logger = logger;

    public async Task<Result<RepairTaskDto>> Handle(
        GetRepairTaskByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var repairTask = await _context.RepairTasks.FindAsync(
            keyValues: [request.Id],
            cancellationToken
        );

        if (repairTask is null)
        {
            _logger.LogWarning("RepairTask with Id '{RepairTaskId}' not found", request.Id);
            return ApplicationErrors.RepairTaskNotFound;
        }

        return repairTask.ToDto();
    }
}
