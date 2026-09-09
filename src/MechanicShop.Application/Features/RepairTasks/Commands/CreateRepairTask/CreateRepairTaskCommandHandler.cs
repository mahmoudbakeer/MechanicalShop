using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public class CreateRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<CreateRepairTaskCommandHandler> logger,
    HybridCache hybridCache
) : IRequestHandler<CreateRepairTaskCommand, Result<RepairTaskDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;

    public async Task<Result<RepairTaskDto>> Handle(
        CreateRepairTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var exist = await _context.RepairTasks.FirstOrDefaultAsync(
            rt => rt.Name == request.Name,
            cancellationToken
        );
        if (exist is not null)
        {
            _logger.LogWarning(
                "Repair task creation aborted. Repair task name {Name} is already exist.",
                request.Name
            );

            return RepairTaskError.RepairTaskAlreadyExist;
        }

        var parts = new List<Part>();
        foreach (var v in request.Parts)
        {
            var result = Part.Create(Guid.NewGuid(), v.Name, v.Quantity, v.Cost);

            if (result.IsError)
            {
                return result.Errors!;
            }

            parts.Add(result.Value);
        }

        var repairTaskResult = RepairTask.Create(
            Guid.NewGuid(),
            request.Name.Trim(),
            request.LaborCost,
            request.Duration,
            parts
        );
        if (repairTaskResult.IsError)
        {
            _logger.LogWarning("Repair task creation aborted. Unknown error occurred.");
            return repairTaskResult.Errors!;
        }
        _context.RepairTasks.Add(repairTaskResult.Value);
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync("repair_tasks", cancellationToken);
        return repairTaskResult.Value.ToDto();
    }
}
