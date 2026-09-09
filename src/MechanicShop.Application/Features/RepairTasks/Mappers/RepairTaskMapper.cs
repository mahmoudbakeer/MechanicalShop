using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;

namespace MechanicShop.Application.Features.RepairTasks.Mappers;

public static class RepairTaskMapper
{
    public static RepairTaskDto ToDto(this RepairTask repairTask)
    {
        return new RepairTaskDto
        {
            Id = repairTask.Id,
            Name = repairTask.Name,
            LaborCost = repairTask.LaborCost,
            EstimatedDuration = repairTask.EstimatedDuration,
            Parts = repairTask.Parts.ToDto(),
        };
    }

    public static PartDto ToDto(this Part part)
    {
        return new PartDto
        {
            Id = part.Id,
            Name = part.Name,
            Quantity = part.Quantity,
            Cost = part.Cost,
        };
    }

    public static List<RepairTaskDto> ToDto(this IEnumerable<RepairTask> repairTasks)
    {
        return [.. repairTasks.Select(r => r.ToDto())];
    }

    public static List<PartDto> ToDto(this IEnumerable<Part> parts)
    {
        return [.. parts.Select(p => p.ToDto())];
    }
}

