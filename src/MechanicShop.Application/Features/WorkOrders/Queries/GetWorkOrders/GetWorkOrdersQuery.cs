using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrders;

public sealed record GetWorkOrdersQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string SortColumn = "createdAt",
    string SortDirection = "desc",
    WorkOrderState? State = null,
    Guid? VehicleId = null,
    Guid? LaborId = null,
    DateTime? StartDateFrom = null,
    DateTime? StartDateTo = null,
    DateTime? EndDateFrom = null,
    DateTime? EndDateTo = null,
    Spot? Spot = null
) : ICachedQuery<Result<PaginatedList<WorkOrderListItemDto>>>
{
    // if any of the parameters are null, we will use "-" as a placeholder in the cache key,
    // the cache will be storing different results for different combinations of parameters,
    // so we need to make sure the cache key is unique for each combination
    public string CacheKey =>
        $"work-orders:p={Page}:ps={PageSize}"
        + $":q={SearchTerm ?? "-"}"
        + $":sort={SortColumn}:{SortDirection}"
        + $":state={State?.ToString() ?? "-"}"
        + $":veh={VehicleId?.ToString() ?? "-"}"
        + $":lab={LaborId?.ToString() ?? "-"}"
        + $":sdfrom={StartDateFrom?.ToString("yyyyMMdd") ?? "-"}"
        + $":sdto={StartDateTo?.ToString("yyyyMMdd") ?? "-"}"
        + $":edfrom={EndDateFrom?.ToString("yyyyMMdd") ?? "-"}"
        + $":edto={EndDateTo?.ToString("yyyyMMdd") ?? "-"}"
        + $":spot={Spot?.ToString() ?? "-"}";

    public string[] Tage => ["work-order"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
