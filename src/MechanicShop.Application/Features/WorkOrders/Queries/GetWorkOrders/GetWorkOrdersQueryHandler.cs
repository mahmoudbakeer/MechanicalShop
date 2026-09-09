using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrders;

public class GetWorkOrdersQueryHandler(IAppDbContext context)
    : IRequestHandler<GetWorkOrdersQuery, Result<PaginatedList<WorkOrderListItemDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PaginatedList<WorkOrderListItemDto>>> Handle(
        GetWorkOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        var workOrders = _context.WorkOrders.AsNoTracking();

        workOrders = ApplyFilters(workOrders, request);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            workOrders = ApplySearchTerm(workOrders, request.SearchTerm);
        var count = await workOrders.CountAsync(cancellationToken); //count after filtering and searching, but before pagination
        workOrders = ApplySorting(workOrders, request.SortColumn, request.SortDirection); // final query after filtering and searching and Sorting

        var items = await workOrders
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(wo => new WorkOrderListItemDto
            {
                WorkOrderId = wo.Id,
                State = wo.State,
                Spot = wo.Spot,
                StartAt = wo.StartedAtUtc,
                EndAt = wo.EndAtUtc,
                Vehicle =
                    wo.Vehicle != null
                        ? new VehicleDto
                        {
                            Id = wo.Vehicle.Id,
                            Make = wo.Vehicle.Make,
                            Model = wo.Vehicle.Model,
                            Year = wo.Vehicle.Year,
                            LicensePlate = wo.Vehicle.LicensePlate,
                        }
                        : null,
                Labor =
                    wo.Employee != null ? wo.Employee.FirstName + " " + wo.Employee.LastName : null,
                Customer =
                    wo.Vehicle != null && wo.Vehicle.Customer != null
                        ? wo.Vehicle.Customer.Name
                        : null,
                RepairTasks = wo.RepairTasks.Select(rt => rt.Name).ToList(),
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<WorkOrderListItemDto>
        {
            Items = items,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalNumber = count,
            TotalPages = (int)Math.Ceiling(count / (double)request.PageSize),
        };
    }

    private static IQueryable<WorkOrder> ApplyFilters(
        IQueryable<WorkOrder> query,
        GetWorkOrdersQuery searchQuery
    )
    {
        if (searchQuery.State.HasValue)
            query = query.Where(wo => wo.State == searchQuery.State);
        if (searchQuery.VehicleId.HasValue && searchQuery.VehicleId != Guid.Empty)
            query = query.Where(wo => wo.VehicleId == searchQuery.VehicleId);
        if (searchQuery.LaborId.HasValue && searchQuery.LaborId != Guid.Empty)
            query = query.Where(wo => wo.EmployeeId == searchQuery.LaborId);
        if (searchQuery.Spot.HasValue)
            query = query.Where(wo => wo.Spot == searchQuery.Spot);
        if (searchQuery.StartDateFrom.HasValue)
            query = query.Where(wo => wo.StartedAtUtc >= searchQuery.StartDateFrom);
        if (searchQuery.EndDateTo.HasValue)
            query = query.Where(wo => wo.EndAtUtc <= searchQuery.EndDateTo);
        if (searchQuery.EndDateFrom.HasValue)
            query = query.Where(wo => wo.EndAtUtc >= searchQuery.EndDateFrom);
        if (searchQuery.StartDateTo.HasValue)
            query = query.Where(wo => wo.StartedAtUtc <= searchQuery.StartDateTo);
        return query;
    }

    private static IQueryable<WorkOrder> ApplySearchTerm(
        IQueryable<WorkOrder> query,
        string searchTerm
    )
    {
        searchTerm = searchTerm.Trim().ToLower();

        query = query.Where(wo =>
            (
                wo.Employee != null
                && (
                    wo.Employee.FirstName.ToLower().Contains(searchTerm)
                    || wo.Employee.LastName.ToLower().Contains(searchTerm)
                    || (wo.Employee.FirstName + " " + wo.Employee.LastName)
                        .ToLower()
                        .Contains(searchTerm)
                )
            )
            || wo.Vehicle != null
                && (
                    wo.Vehicle.LicensePlate.ToLower().Contains(searchTerm)
                    || wo.Vehicle.Make.ToLower().Contains(searchTerm)
                    || wo.Vehicle.Model.ToLower().Contains(searchTerm)
                    || wo.Vehicle.Year.ToString().Contains(searchTerm)
                )
            || (
                wo.RepairTasks != null
                && wo.RepairTasks.Any(rt => rt.Name.ToLower().Contains(searchTerm))
            )
            || wo.Id.ToString().Contains(searchTerm)
        );
        return query;
    }

    private static IQueryable<WorkOrder> ApplySorting(
        IQueryable<WorkOrder> query,
        string sortColumn,
        string sortDirection
    )
    {
        var descending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortColumn.ToLowerInvariant() switch
        {
            "createdat" => descending
                ? query.OrderByDescending(wo => wo.CreatedAtUtc)
                : query.OrderBy(wo => wo.CreatedAtUtc),

            "updatedat" => descending
                ? query.OrderByDescending(wo => wo.LastModifiedUtc)
                : query.OrderBy(wo => wo.LastModifiedUtc),

            "startat" => descending
                ? query.OrderByDescending(wo => wo.StartedAtUtc)
                : query.OrderBy(wo => wo.StartedAtUtc),

            "endat" => descending
                ? query.OrderByDescending(wo => wo.EndAtUtc)
                : query.OrderBy(wo => wo.EndAtUtc),

            "state" => descending
                ? query.OrderByDescending(wo => wo.State)
                : query.OrderBy(wo => wo.State),

            "spot" => descending
                ? query.OrderByDescending(wo => wo.Spot)
                : query.OrderBy(wo => wo.Spot),

            "total" => descending
                ? query.OrderByDescending(wo => wo.Total)
                : query.OrderBy(wo => wo.Total),

            "vehicleid" => descending
                ? query.OrderByDescending(wo => wo.VehicleId)
                : query.OrderBy(wo => wo.VehicleId),

            "laborid" => descending
                ? query.OrderByDescending(wo => wo.EmployeeId)
                : query.OrderBy(wo => wo.EmployeeId),

            _ => query.OrderByDescending(wo => wo.CreatedAtUtc),
        };
    }
}
