using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.RepairTasks;

public class UpdateRepairTaskRequest
{
    [Required(ErrorMessage = "Name is required.")]
    [Length(
        1,
        100,
        ErrorMessage = "Name must have at least one character and maximum 100 characters."
    )]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Duration is required.")]
    [EnumDataType(typeof(RepairTaskDuration))]
    public RepairTaskDuration Duration { get; set; }

    [Required(ErrorMessage = "Labor cost is required.")]
    [Range(1, 10000, ErrorMessage = "Labor cost must be between 1 and 10,000.")]
    public decimal LaborCost { get; set; }

    [Required(ErrorMessage = "Parts is required.")]
    [Length(
        1,
        100,
        ErrorMessage = "At least one part is required and maximun 100 parts is allowed."
    )]
    public List<UpdatePartRequest> Parts { get; set; } = null!;
}

public class UpdatePartRequest
{
    [Required(ErrorMessage = "PartId is required.")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [Length(
        1,
        100,
        ErrorMessage = "Name must have at least one character and maximum 100 characters."
    )]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Cost is required.")]
    [Range(1, 10000, ErrorMessage = "Cost must be between 1 and 10,000.")]
    public decimal Cost { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
    public int Quantity { get; set; }
}
