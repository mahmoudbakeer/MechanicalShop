using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(int Page, int PageSize)
    : ICachedQuery<Result<PaginatedList<CustomerDto>>>
{
    public string CacheKey => "customers";

    public string[] Tags => ["customer"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
