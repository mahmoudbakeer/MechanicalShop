using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public class WorkOrdersFilter
{
    public string? SearchTerm;
    public string SortColumn = "createdAt";
    public string SortDirection = "desc";
    public WorkOrderState? State = null;
    public Guid? VehicleId = null;
    public Guid? LaborId = null;
    public DateTime? StartDateFrom = null;
    public DateTime? StartDateTo = null;
    public DateTime? EndDateFrom = null;
    public DateTime? EndDateTo = null;
    public Spot? Spot = null;
}
