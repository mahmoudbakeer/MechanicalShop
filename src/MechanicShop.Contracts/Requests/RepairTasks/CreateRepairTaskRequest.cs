using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.RepairTasks;

public class CreateRepairTaskRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name can not be more the 100 characters.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Duration is required.")]
    [EnumDataType(typeof(RepairTaskDuration))]
    public RepairTaskDuration Duration { get; set; }

    [Required(ErrorMessage = "LaborCost is required.")]
    [Range(1, 10000, ErrorMessage = "LaborCost at least 1$ and maximum 10,000$.")]
    public decimal LaborCost { get; set; }

    [Required(ErrorMessage = "Parts is required.")]
    [Length(1, 100, ErrorMessage = "At least one part is required for repairTask.")]
    public List<CreatePartRequest> Parts { get; set; } = null!;
}

public class CreatePartRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name can not be more the 100 characters.")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Cost is required.")]
    [Range(1, 10000, ErrorMessage = "LaborCost at least 1$ and maximum 10,000$.")]
    public decimal Cost { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "Quantity must be at least one and maximum 100.")]
    public int Quantity { get; set; }
}
