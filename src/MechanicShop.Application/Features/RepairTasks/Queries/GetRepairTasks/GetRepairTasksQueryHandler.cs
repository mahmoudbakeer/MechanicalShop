using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;

public sealed class GetRepairTasksQueryHandler(
    IAppDbContext context,
    ILogger<GetRepairTasksQueryHandler> logger
) : IRequestHandler<GetRepairTasksQuery, Result<PaginatedList<RepairTaskDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetRepairTasksQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<RepairTaskDto>>> Handle(
        GetRepairTasksQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Fetching RepairTasks started, Page '{PageNumber}' PageSize '{PageSize}'",
            request.Page,
            request.PageSize
        );
        var repairTasks = await _context
            .RepairTasks.OrderBy(rt => rt.Id)
            .AsNoTracking()
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await _context.RepairTasks.CountAsync(cancellationToken);
        var paginatedList = new PaginatedList<RepairTaskDto>
        {
            Items = repairTasks.ToDto(),
            PageSize = request.PageSize,
            PageNumber = request.Page,
            TotalNumber = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
        };

        return paginatedList;
    }
}
