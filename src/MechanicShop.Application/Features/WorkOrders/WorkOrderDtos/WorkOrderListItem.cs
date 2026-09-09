using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;

public class WorkOrderListItemDto
{
    public Guid WorkOrderId { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? Customer { get; set; }
    public string? Labor { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public WorkOrderState State { get; set; }
    public Spot Spot { get; set; }
    public List<string>? RepairTasks { get; set; }
}
