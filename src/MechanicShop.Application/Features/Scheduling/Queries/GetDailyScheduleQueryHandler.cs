using System.ComponentModel;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Application.Features.Scheduling.SchedulingDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Scheduling.Queries;

public class GetDailyScheduleQueryHandler(IAppDbContext context, TimeProvider timeProvider)
    : IRequestHandler<GetDailyScheduleQuery, Result<ScheduleDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<ScheduleDto>> Handle(
        GetDailyScheduleQuery request,
        CancellationToken cancellationToken
    )
    {
        var localStart = request.ScheduleDate.ToDateTime(TimeOnly.MinValue);
        var localEnd = localStart.AddDays(1);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(localStart, request.TimeZoneInfo);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(localEnd, request.TimeZoneInfo);

        var workOrders = await _context
            .WorkOrders.Where(wo =>
                wo.StartedAtUtc < endUtc
                && wo.EndAtUtc > startUtc
                && (request.LaborId == null || wo.EmployeeId == request.LaborId)
            )
            .Include(wo => wo.Employee)
            .Include(wo => wo.Vehicle)
            .Include(wo => wo.RepairTasks)
            .ToListAsync(cancellationToken);

        var now = TimeZoneInfo.ConvertTime(_timeProvider.GetUtcNow(), request.TimeZoneInfo);
        var spots = new List<SpotDto>();

        foreach (var spot in Enum.GetValues<Spot>())
        {
            var current = localStart;
            var slots = new List<AvailabilitySlotDto>(); // for each spot list of slots from the start of the day
            var woBySpot = workOrders.Where(wo => wo.Spot == spot).OrderBy(wo => wo.StartedAtUtc);

            while (current < localEnd) // from start of the day till the end.
            {
                var next = current.AddMinutes(15); // each slot 15 minutes.

                var slotStartUtc = TimeZoneInfo.ConvertTimeToUtc(current, request.TimeZoneInfo);
                var slotEndUtc = TimeZoneInfo.ConvertTimeToUtc(next, request.TimeZoneInfo);

                var wo = woBySpot.FirstOrDefault(wo =>
                    wo.StartedAtUtc < slotEndUtc && wo.EndAtUtc > slotStartUtc
                );

                if (wo is not null)
                {
                    if (!slots.Any(s => s.WorkOrderId == wo.Id))
                    {
                        var slot = new AvailabilitySlotDto
                        {
                            Spot = spot,
                            StartAt = wo.StartedAtUtc,
                            EndAt = wo.EndAtUtc,
                            IsAvailable = false,
                            IsOccupied = true,
                            WorkOrderLocked = true,
                            WorkOrderId = wo.Id,
                            Vehicle = wo.Vehicle is not null ? FormateVehicle(wo.Vehicle) : null,
                            Labor = wo.Employee?.ToDto(),
                            RepairTasks = wo.RepairTasks is not null
                                ? [.. wo.RepairTasks.ToDto()]
                                : [],
                            State = wo.State,
                        };

                        slots.Add(slot);
                    }
                }
                else
                {
                    var slot = new AvailabilitySlotDto
                    {
                        Spot = spot,
                        StartAt = current,
                        EndAt = next,
                        IsAvailable = current >= now.DateTime, // if the current means the slot time in the future
                        IsOccupied = false,
                        WorkOrderLocked = false,
                    };
                    slots.Add(slot);
                }
                current = current.AddMinutes(15);
            }
            var spotDto = new SpotDto { Spot = spot, Slots = slots };
            spots.Add(spotDto);
        }

        var schedule = new ScheduleDto
        {
            Spots = spots,
            Date = request.ScheduleDate,
            EndOfDay = localEnd <= now.DateTime,
        };

        return schedule;
    }

    private static string FormateVehicle(Vehicle? vehicle)
    {
        return vehicle is not null ? $"{vehicle.Make} | {vehicle.LicensePlate}" : "";
    }
}
