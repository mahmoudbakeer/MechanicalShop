using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Labors.Queries;

public sealed record GetEmployeesQuery(int Page, int PageSize)
    : ICachedQuery<Result<PaginatedList<LaborDto>>>
{
    public string CacheKey => "Employees";

    public string[] Tage => ["Employees"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
