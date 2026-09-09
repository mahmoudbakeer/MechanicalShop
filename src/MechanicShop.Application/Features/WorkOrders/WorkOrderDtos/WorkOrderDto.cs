using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;
using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;

public class WorkOrderDto
{
    public Guid Id { get; set; }
    public Guid? InvoiceId { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Spot Spot { get; set; }
    public List<RepairTaskDto> RepairTasks { get; set; } = [];
    public int DurationInMinutes { get; set; }
    public decimal TotalLaborCost { get; set; }
    public decimal TotalPartsCost { get; set; }
    public decimal Total { get; set; }
    public WorkOrderState State { get; set; }
    public LaborDto? Labor { get; set; }
    public VehicleDto? Vehicle { get; set; }
}
