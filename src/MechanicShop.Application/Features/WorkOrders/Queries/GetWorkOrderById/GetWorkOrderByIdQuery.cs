using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById;

public sealed record GetWorkOrderByIdQuery(Guid WorkOrderId) : ICachedQuery<Result<WorkOrderDto>>
{
    public string CacheKey => $"work-order_{WorkOrderId}";

    public string[] Tage => ["work-order"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
