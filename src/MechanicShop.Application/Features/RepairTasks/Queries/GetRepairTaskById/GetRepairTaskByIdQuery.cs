using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;

public sealed record GetRepairTaskByIdQuery(Guid Id) : ICachedQuery<Result<RepairTaskDto>>
{
    public string CacheKey => $"repair_task_{Id}";

    public string[] Tage => ["repair_task"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
