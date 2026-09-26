using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Features.Scheduling.SchedulingDtos;

public sealed class AvailabilitySlotDto
{
    public Guid? WorkOrderId { get; set; }
    public Spot Spot { get; set; }
    public DateTimeOffset StartAt { get; set; }
    public DateTimeOffset EndAt { get; set; }
    public string? Vehicle { get; set; }
    public LaborDto? Labor { get; set; }
    public bool IsOccupied { get; set; }
    public bool? IsAvailable { get; set; }
    public bool WorkOrderLocked { get; set; }
    public WorkOrderState? State { get; set; }
    public RepairTaskDto[]? RepairTasks { get; set; }
}