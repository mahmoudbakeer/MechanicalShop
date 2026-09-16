using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Events;
using MechanicShop.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MechanicShop.Infrastructure.Services;

public sealed class WorkOrderPolicy(IOptions<AppSettings> options, IAppDbContext context)
    : IWorkOrderPolicy
{
    private readonly AppSettings _appSettings = options.Value;
    private readonly IAppDbContext _context = context;

    public async Task<bool> IsLaborOccupiedAsync(
        Guid LaborId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct
    )
    {
        // The new interval starts before the old one ends, and ends after the old one starts. then conflict
        return await _context.WorkOrders.AnyAsync(
            wo => wo.EmployeeId == LaborId && wo.StartedAtUtc < endTime && wo.EndAtUtc > startTime,
            ct
        );
    }

    public bool IsOutsideBusinessHours(DateTimeOffset startedAtUtc, TimeSpan totalDuration)
    {
        var openingTime = startedAtUtc.Date.Add(_appSettings.OpeningTime.ToTimeSpan());

        var closingTime = startedAtUtc.Date.Add(_appSettings.ClosingTime.ToTimeSpan());

        var endingAt = startedAtUtc.Add(totalDuration);

        return startedAtUtc < openingTime || endingAt > closingTime;
    }

    public async Task<bool> IsSpotAvailableAsync(
        Spot spot,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct,
        Guid? ExcludeWorkOrderId = null
    )
    {
        return !await _context.WorkOrders.AnyAsync(
            wo =>
                wo.Spot == spot
                && wo.StartedAtUtc < endTime
                && wo.EndAtUtc > startTime
                && (ExcludeWorkOrderId == null || wo.Id != ExcludeWorkOrderId),
            ct
        );
    }

    public async Task<bool> IsVehicleAlreadyScheduledAsync(
        Guid VehicleId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        CancellationToken ct,
        Guid? ExcludeWorkOrderId = null
    )
    {
        return await _context.WorkOrders.AnyAsync(
            wo =>
                wo.VehicleId == VehicleId
                && wo.StartedAtUtc < endTime
                && wo.EndAtUtc > startTime
                && (ExcludeWorkOrderId == null || wo.Id != ExcludeWorkOrderId),
            ct
        );
    }

    public Result<Success> ValidateMinimumRequirement(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        if (
            endAt - startAt
            < TimeSpan.FromMinutes(_appSettings.MinimumAppointmentDurationInMinutes)
        )
        {
            return Error.Conflict(
                "WorkOrder_TooShort",
                "The work order duration is less than the minimum required duration."
            );
        }
        return Result.Success;
    }
}
