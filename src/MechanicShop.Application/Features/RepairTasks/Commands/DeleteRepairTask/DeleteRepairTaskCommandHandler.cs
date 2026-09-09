using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.RepairTasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Commands.DeleteRepairTask;

public class DeleteRepairTaskCommandHandler(
    IAppDbContext context,
    ILogger<DeleteRepairTaskCommandHandler> logger,
    HybridCache hybridCache
) : IRequestHandler<DeleteRepairTaskCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<DeleteRepairTaskCommandHandler> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;

    async Task<Result<Deleted>> IRequestHandler<DeleteRepairTaskCommand, Result<Deleted>>.Handle(
        DeleteRepairTaskCommand request,
        CancellationToken cancellationToken
    )
    {
        var exist = await _context.RepairTasks.FindAsync(
            [request.Id],
            cancellationToken: cancellationToken
        );
        if (exist is null)
        {
            _logger.LogWarning(
                "Repair task deletion aborted. RepairTask with id '{RepairTaskId}' does not exist.",
                request.Id
            );

            return ApplicationErrors.RepairTaskNotFound;
        }
        var isInUse = await _context
            .WorkOrders.AsNoTracking()
            .SelectMany(x => x.RepairTasks)
            .AnyAsync(rt => rt.Id == request.Id, cancellationToken);

        if (isInUse)
        {
            _logger.LogWarning(
                "RepairTask {RepairTaskId} cannot be deleted — in use by work orders.",
                request.Id
            );

            return RepairTaskError.InUse;
        }

        _context.RepairTasks.Remove(exist);
        await _context.SaveChangesAsync(cancellationToken);
        await _hybridCache.RemoveAsync("repair_tasks", cancellationToken);
        return Result.Deleted;
    }
}
