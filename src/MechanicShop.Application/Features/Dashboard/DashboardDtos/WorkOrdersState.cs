namespace MechanicShop.Application.Features.Dashboard.DashboardDtos;

public class WorkOrdersState
{
    public int TotalOrders { get; set; }
    public int ScheduledOrders { get; set; }
    public int InProgressOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public int UniqueVehcles { get; set; }
    public int UniqueCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalPartsCost { get; set; }
    public decimal NetProfit { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal ProfitMargin { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal AverageRevenuePerOrder { get; set; }
    public decimal OrdersPerVehicle { get; set; }
    public decimal PartsCostRatio { get; set; }
    public decimal LaborCostRatio { get; set; }
    public decimal CancellationRate { get; set; }
    public DateOnly Date { get; set; }
}
