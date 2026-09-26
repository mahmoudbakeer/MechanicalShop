using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;


public class RelocateWorkOrderRequest
{
    [Required(ErrorMessage = "Spot is required.")]
    [EnumDataType(typeof(Spot))]
    public Spot Spot { get; set; }
    [Required(ErrorMessage = "StartAt required.")]
    public DateTimeOffset StartAt { get; set; }
}