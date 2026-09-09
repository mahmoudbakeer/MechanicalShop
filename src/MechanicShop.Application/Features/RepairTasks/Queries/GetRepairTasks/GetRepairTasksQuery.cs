using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;

public sealed record GetRepairTasksQuery(int Page, int PageSize)
    : ICachedQuery<Result<PaginatedList<RepairTaskDto>>>
{
    public string CacheKey => "repair_tasks";

    public string[] Tage => ["repair_tasks"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
