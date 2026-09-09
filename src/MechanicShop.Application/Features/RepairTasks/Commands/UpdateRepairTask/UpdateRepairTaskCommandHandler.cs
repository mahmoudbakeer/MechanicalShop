using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;

public class UpdateRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<UpdateRepairTaskCommandHandler> logger,
    HybridCache hybridCache
) : IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdateRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;

    async Task<Result<Updated>> IRequestHandler<UpdateRepairTaskCommand, Result<Updated>>.Handle(
        UpdateRepairTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var exist = await _context.RepairTasks.FirstOrDefaultAsync(
            rt => rt.Name == request.Name,
            cancellationToken
        );
        if (exist is null)
        {
            _logger.LogWarning(
                "Repair task Updation aborted. RepairTask with id '{RepairTaskId}' does not exist.",
                request.Id
            );

            return ApplicationErrors.RepairTaskNotFound;
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

        var repairTaskResult = exist.Update(
            request.Name.Trim(),
            request.LaborCost,
            request.Duration
        );
        if (repairTaskResult.IsError)
        {
            _logger.LogWarning("Repair task Updation aborted. Unknown error occurred.");
            return repairTaskResult.Errors!;
        }

        var upsertingpartsResult = exist.UpsertParts(parts);
        if (upsertingpartsResult.IsError)
        {
            _logger.LogWarning("Repair task Updation aborted. Unknown error occurred.");
            return upsertingpartsResult.Errors!;
        }
        await _hybridCache.RemoveAsync("repair_tasks", cancellationToken);
        return Result.Updated;
    }
}
