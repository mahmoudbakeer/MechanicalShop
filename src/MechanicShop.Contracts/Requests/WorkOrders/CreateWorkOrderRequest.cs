using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;


public class CreateWorkOrderRequest
{
    [Required(ErrorMessage = "LaborId is required.")]
    public Guid LaborId { get; set; }
    [Required(ErrorMessage = "VehicleId is required.")]
    public Guid VehicleId { get; set; }
    [Required(ErrorMessage = "StartAt is required.")]
    public DateTimeOffset StartAt { get; set; }
    [Required(ErrorMessage = "Spot is required.")]
    [EnumDataType(typeof(Spot))]
    public Spot Spot { get; set; }
    [Required(ErrorMessage = "RepairTaskIds is required.")]
    [MinLength(1, ErrorMessage = "Required at least one RepairTask.")]
    public List<Guid> RepairTaskIds { get; set; } = null!;
}