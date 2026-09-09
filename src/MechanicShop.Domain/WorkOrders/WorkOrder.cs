using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Domain.WorkOrders;

public class WorkOrder : AuditableEntity
{
    private readonly List<RepairTask> _repairTasks = [];
    public IEnumerable<RepairTask> RepairTasks => _repairTasks.AsReadOnly();
    public Employee Employee { get; set; }
    public Guid EmployeeId { get; private set; }
    public Vehicle Vehicle { get; set; }
    public Guid VehicleId { get; private set; }
    public Invoice? Invoice { get; set; }
    public Guid InvoiceId { get; private set; }
    public DateTimeOffset StartedAtUtc { get; private set; }
    public DateTimeOffset EndAtUtc { get; private set; }
    public Spot Spot { get; private set; }
    public WorkOrderState State { get; private set; }
    public decimal TotalPartsCost => _repairTasks.SelectMany(rp => rp.Parts).Sum(p => p.Cost);
    public decimal TotalLaborCost => _repairTasks.Sum(rp => rp.LaborCost);
    public decimal Total => TotalPartsCost + TotalLaborCost;
    public decimal? Discount { get; private set; }

#pragma warning disable CS8618
    private WorkOrder() { }
#pragma warning disable CS8618

    private WorkOrder(
        Guid id,
        Guid employeeId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        Guid vehicleId,
        Spot spot,
        List<RepairTask> repairTasks
    )
        : base(id)
    {
        EmployeeId = employeeId;
        StartedAtUtc = startAt;
        EndAtUtc = endAt;
        VehicleId = vehicleId;
        _repairTasks = repairTasks;
        Spot = spot;

        State = WorkOrderState.Scheduled;
    }

    public static Result<WorkOrder> Create(
        Guid id,
        Guid employeeId,
        DateTimeOffset startAt,
        DateTimeOffset endAt,
        Guid vehicleId,
        Spot spot,
        List<RepairTask> repairTasks,
        DateTimeOffset Now
    )
    {
        if (Guid.Empty == id)
            return WorkOrderError.WorkOrderIdRequired;
        if (Guid.Empty == employeeId)
            return WorkOrderError.LaborIdRequired;
        if (Guid.Empty == vehicleId)
            return WorkOrderError.VehicleIdRequired;
        if (!System.Enum.IsDefined(spot))
            return WorkOrderError.InvalidSpot;
        if (startAt.Day < Now.Day)
            return WorkOrderError.InvalidStartTime;
        if (startAt >= endAt)
            return WorkOrderError.InvalidEndTime;
        if (repairTasks is null || repairTasks.Count == 0)
            return WorkOrderError.RepairTasksRequired;

        return new WorkOrder(id, employeeId, startAt, endAt, vehicleId, spot, repairTasks);
    }

    public bool IsEditable => State == WorkOrderState.Scheduled;

    public Result<Updated> UpdateTiming(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (!IsEditable)
            return WorkOrderError.TimingReadOnly(Id, State);
        if (startAt >= endAt)
            return WorkOrderError.ReadOnly;
        StartedAtUtc = startAt;
        EndAtUtc = endAt;

        return Result.Updated;
    }

    public Result<Success> UpdateLabor(Guid employeeId)
    {
        if (!IsEditable)
            return WorkOrderError.ReadOnly;

        if (Guid.Empty == employeeId)
            return WorkOrderError.LaborIdRequired;

        EmployeeId = employeeId;
        return Result.Success;
    }

    public Result<Success> AddRepairTask(RepairTask repairTask)
    {
        if (!IsEditable)
            return WorkOrderError.ReadOnly;
        if (_repairTasks.Any(rt => repairTask.Id == rt.Id))
            return WorkOrderError.RepairTasksAlreadyExist;

        _repairTasks.Add(repairTask);
        return Result.Success;
    }

    public bool CanTransferTo(WorkOrderState newState)
    {
        if (!IsEditable)
            return false;
        else
        {
            return (State, newState) switch
            {
                (WorkOrderState.Scheduled, WorkOrderState.InProgress) => true,
                (WorkOrderState.InProgress, WorkOrderState.Completed) => true,
                (_, WorkOrderState.Cancelled) when State != WorkOrderState.InProgress => true,
                _ => false,
            };
        }
    }

    public Result<Success> UpdateState(WorkOrderState newState)
    {
        if (!IsEditable)
            return WorkOrderError.StateTransformationNotAllowed(StartedAtUtc);
        if (!CanTransferTo(newState))
            return WorkOrderError.InvalidStateTransformation(State, newState);

        State = newState;

        return Result.Success;
    }

    public Result<Success> Cancel()
    {
        return UpdateState(WorkOrderState.Cancelled);
    }

    public Result<Updated> ClearRepairTasks()
    {
        if (!IsEditable)
            return WorkOrderError.ReadOnly;
        _repairTasks.Clear();
        return Result.Updated;
    }

    public Result<Success> UpdateSpot(Spot newSpot)
    {
        if (!IsEditable)
            return WorkOrderError.ReadOnly;
        if (!System.Enum.IsDefined(newSpot))
            return WorkOrderError.InvalidSpot;

        Spot = newSpot;
        return Result.Success;
    }
}
