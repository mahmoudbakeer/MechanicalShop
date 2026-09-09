using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.WorkOrders;

namespace MechanicShop.Application.Features.WorkOrders.WorkOrderMappers;

public static class WorkOrderMappers
{
    public static WorkOrderDto ToDto(this WorkOrder workOrder)
    {
        return new WorkOrderDto
        {
            Id = workOrder.Id,
            Spot = workOrder.Spot,
            StartAt = workOrder.StartedAtUtc,
            EndAt = workOrder.EndAtUtc,
            Labor = workOrder.Employee is null
                ? null
                : new LaborDto
                {
                    Id = workOrder.EmployeeId,
                    Name = $"{workOrder.Employee.FirstName} {workOrder.Employee.LastName}",
                },
            RepairTasks = workOrder.RepairTasks.ToDto(),
            Vehicle = workOrder.Vehicle.ToDto(),
            State = workOrder.State,
            TotalPartsCost = workOrder
                .RepairTasks.SelectMany(t => t.Parts)
                .Sum(p => p.Cost * p.Quantity),
            TotalLaborCost = workOrder.RepairTasks.Sum(p => p.LaborCost),
            Total = workOrder.RepairTasks.Sum(rt => rt.TotalCost),
            DurationInMinutes = workOrder.RepairTasks.Sum(rt => (int)rt.EstimatedDuration),
            InvoiceId = workOrder.Invoice?.Id,
            CreatedAt = workOrder.CreatedAtUtc,
        };
    }

    public static WorkOrderListItemDto ToListItemDto(this WorkOrder workOrder)
    {
        return new WorkOrderListItemDto
        {
            WorkOrderId = workOrder.Id,
            Spot = workOrder.Spot,
            StartAt = workOrder.StartedAtUtc,
            EndAt = workOrder.EndAtUtc,
            Labor = workOrder.Employee is null
                ? null
                : $"{workOrder.Employee.FirstName} {workOrder.Employee.LastName}",
            RepairTasks = workOrder.RepairTasks.Select(rt => rt.Name).ToList(),
            Vehicle = workOrder.Vehicle.ToDto(),
            State = workOrder.State,
            InvoiceId = workOrder.Invoice?.Id,
            Customer = workOrder.Vehicle.Customer.Name,
        };
    }

    public static List<WorkOrderListItemDto> ToListItemDto(this IEnumerable<WorkOrder> workOrders)
    {
        return [.. workOrders.Select(w => w.ToListItemDto())];
    }

    public static List<WorkOrderDto> ToDto(this IEnumerable<WorkOrder> workOrders)
    {
        return [.. workOrders.Select(w => w.ToDto())];
    }
}
