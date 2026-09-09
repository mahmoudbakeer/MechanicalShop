namespace MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;

public class PartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Cost { get; set; }
    public int Quantity { get; set; }
}

