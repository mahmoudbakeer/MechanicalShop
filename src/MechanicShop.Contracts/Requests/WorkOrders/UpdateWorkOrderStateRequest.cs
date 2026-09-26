using System.ComponentModel.DataAnnotations;
using MechanicShop.Contracts.Common;

namespace MechanicShop.Contracts.Requests.WorkOrders;


public class UpdateWorkOrderStateRequest
{

    [Required(ErrorMessage = "WorkOrderState is required.")]
    [EnumDataType(typeof(WorkOrderState))]
    public WorkOrderState NewState { get; set; }
}