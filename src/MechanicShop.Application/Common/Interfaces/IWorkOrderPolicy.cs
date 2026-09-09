using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;

namespace MechanicShop.Application.Common.Interfaces;

public interface IWorkOrderPolicy
{
    Task<bool> IsLaborOccupiedAsync(
        Guid LaborId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct
    );
    Task<bool> IsVehicleAlreadyScheduledAsync(
        Guid VehicleId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct,
        Guid? ExcludeWorkOrderId = default
    );

    Task<bool> IsSpotAvailableAsync(
        Spot spot,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct,
        Guid? ExcludeWorkOrderId = default
    );
    Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt);
    bool IsOutsideBusinessHours(DateTimeOffset startedAtUtc, TimeSpan totalDuration);
}
