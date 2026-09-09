using MechanicShop.Domain.RepairTasks.Enum;

namespace MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;

public class RepairTaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal LaborCost { get; set; }
    public RepairTaskDuration EstimatedDuration { get; set; }
    public List<PartDto> Parts { get; set; } = default!;
}
