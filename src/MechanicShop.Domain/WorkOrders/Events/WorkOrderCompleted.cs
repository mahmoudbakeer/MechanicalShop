using MechanicShop.Domain.Common;

namespace MechanicShop.Domain.WorkOrder.Events;



public sealed class WorkOrderCompleted : DomainEvent
{
    public Guid WorkOrderId { get; set; }
}