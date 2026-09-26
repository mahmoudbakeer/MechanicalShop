using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Scheduling.SchedulingDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Scheduling.Queries;


public sealed record GetDailyScheduleQuery(TimeZoneInfo TimeZoneInfo, DateOnly ScheduleDate, Guid? LaborId) : ICachedQuery<Result<ScheduleDto>>
{
    public string CacheKey => $"schedule_{ScheduleDate}";

    public string[] Tags => ["schedule"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(30);
}