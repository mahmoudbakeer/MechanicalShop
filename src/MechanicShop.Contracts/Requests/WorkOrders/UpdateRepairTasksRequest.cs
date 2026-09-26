using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.WorkOrders;

public class UpdateRepairTasksRequest
{
    [Required(ErrorMessage = "RepairTasks is required")]
    [MinLength(1, ErrorMessage = "Require at least one repairTask.")]
    public Guid[] RepairTasks { get; set; } = null!;
}