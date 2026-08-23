using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enum;

namespace MechanicShop.Domain.WorkOrders;


public static class WorkOrderError
{
    public static Error WorkOrderIdRequired => Error.Validation("WorkOrder_Id_Required.", "WorkOrder Id cannot be null or empty.");
    public static Error VehicleIdRequired => Error.Validation("WorkOrder_Vehicle_Id_Required.", "Vehicle Id cannot be null or empty.");
    public static Error LaborIdRequired => Error.Validation("WorkOrder_Employee_Id_Required.", "Labor Id cannot be null or empty.");
    public static Error InvalidSpot => Error.Validation("WorkOrder_Spot_Invalid.", "Spot Invalid.");
    public static Error InvalidStartTime => Error.Validation("WorkOrder_StartTime_Invalid.", "Start time must be in the future.");
    public static Error InvalidEndTime => Error.Validation("WorkOrder_EndTime_Invalid.", "EndTime must be after start time.");
    public static Error ReadOnly => Error.Conflict("WorkOrder_ReadOnly.", "WorkOrder is read-only.");
    public static Error TimingReadOnly(Guid id, WorkOrderState newState) => Error.Conflict("WorkOrder_Timing_ReadOnly.", $"WorkOrder id : {id}, cannot modify timing when the workOrder state is {newState}");
    public static Error StateTransformationNotAllowed(DateTimeOffset startTime) => Error.Conflict("WorkOrder_State_Transformation_NotAllowed", $"State transformation not allowed before workOrder's starting time {startTime}.");
    public static Error InvalidStateTransformation(WorkOrderState oldState, WorkOrderState newState) => Error.Conflict("WorkOrder_State_Transformation_Invalid.", $"Cannot change the State of workOrder from {oldState} to {newState}.");
    public static Error RepairTasksRequired => Error.Validation("WorkOrder_RepairTasks_Required.", "RepairTasks is null or empty at least on task is required.");
    public static Error RepairTasksAlreadyExist => Error.Conflict
    ("WorkOrder_RepairTasks_AlreadyExist.", "RepairTask is already added.");
}