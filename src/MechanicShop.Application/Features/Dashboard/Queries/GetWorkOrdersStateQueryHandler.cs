using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Dashboard.DashboardDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Application.Features.Dashboard.Queries;

public class GetWorkOrdersStateQueryHandler(IAppDbContext context)
    : IRequestHandler<GetWorkOrdersStateQuery, Result<WorkOrdersState>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<WorkOrdersState>> Handle(
        GetWorkOrdersStateQuery request,
        CancellationToken cancellationToken
    )
    {
        var startAt = request.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endAt = request.Date.AddDays(1).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
        var query = _context
            .WorkOrders.AsNoTracking()
            .Include(wo => wo.Vehicle)
                .ThenInclude(v => v.Customer)
            .Include(wo => wo.RepairTasks)
                .ThenInclude(rt => rt.Parts)
            .Include(wo => wo.Invoice)
                .ThenInclude(i => i!.LineItems) // here there is potentioal null reference exception if Invoice is null, but include is an instruction to run on database side, so it will not throw exception,
                                                // but it will not include InvoiceLineItems if Invoice is null and no annoying warning will be appearing here
            .Where(wo => wo.StartedAtUtc >= startAt && wo.StartedAtUtc <= endAt);

        var totalOrders = await query.CountAsync(cancellationToken);
        if (totalOrders == 0)
        {
            return new WorkOrdersState
            {
                Date = request.Date,
                TotalOrders = 0,
                ScheduledOrders = 0,
                InProgressOrders = 0,
                CompletedOrders = 0,
                CancelledOrders = 0,
                UniqueVehcles = 0,
                UniqueCustomers = 0,
                TotalRevenue = 0,
                TotalPartsCost = 0,
                NetProfit = 0,
                TotalLaborCost = 0,
                ProfitMargin = 0,
                CompletionRate = 0,
                AverageRevenuePerOrder = 0,
                OrdersPerVehicle = 0,
                PartsCostRatio = 0,
                LaborCostRatio = 0,
                CancellationRate = 0,
            };
        }
        var stats = await query.ToListAsync(cancellationToken);
        var scheduledOrders = stats.Count(wo => wo.State == WorkOrderState.Scheduled);
        var inProgressOrders = stats.Count(wo => wo.State == WorkOrderState.InProgress);
        var completedOrders = stats.Count(wo => wo.State == WorkOrderState.Completed);
        var cancelledOrders = stats.Count(wo => wo.State == WorkOrderState.Cancelled);
        var uniqueVehicle = stats.Select(wo => wo.Vehicle?.Id).Distinct().Count();
        var uniqueCustomer = stats.Select(wo => wo.Vehicle?.Customer?.Id).Distinct().Count();
        var totalRevenue = stats.Sum(wo => wo.Invoice?.Total ?? 0m);
        var totalPartsCost = stats.Sum(wo => wo.RepairTasks.Sum(rt => rt.Parts.Sum(p => p.Cost)));
        var totalLaborCost = stats.Sum(wo => wo.RepairTasks.Sum(rt => rt.LaborCost));
        var netProfit = totalRevenue - totalPartsCost - totalLaborCost;

        return new WorkOrdersState
        {
            Date = request.Date,
            TotalOrders = totalOrders,
            ScheduledOrders = scheduledOrders,
            InProgressOrders = inProgressOrders,
            CompletedOrders = completedOrders,
            CancelledOrders = cancelledOrders,
            UniqueVehcles = uniqueVehicle,
            UniqueCustomers = uniqueCustomer,
            TotalRevenue = totalRevenue,
            TotalPartsCost = totalPartsCost,
            NetProfit = netProfit,
            TotalLaborCost = totalLaborCost,
            ProfitMargin = totalRevenue == 0 ? 0 : netProfit / totalRevenue * 100,
            CompletionRate = totalOrders == 0 ? 0 : (decimal)completedOrders / totalOrders * 100,
            AverageRevenuePerOrder = totalOrders == 0 ? 0 : totalRevenue / totalOrders,
            OrdersPerVehicle = uniqueVehicle == 0 ? 0 : (decimal)totalOrders / uniqueVehicle,
            PartsCostRatio = totalRevenue == 0 ? 0 : totalPartsCost / totalRevenue * 100,
            LaborCostRatio = totalRevenue == 0 ? 0 : totalLaborCost / totalRevenue * 100,
            CancellationRate = totalOrders == 0 ? 0 : (decimal)cancelledOrders / totalOrders * 100,
        };
    }
}
