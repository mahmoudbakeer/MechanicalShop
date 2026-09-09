using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Constants;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Billing;
using MechanicShop.Domain.WorkOrders.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.IssueInvoice;

public sealed class IssueInvoiceCommandHandler(
    IAppDbContext context,
    ILogger<IssueInvoiceCommandHandler> logger,
    TimeProvider timeProvider,
    HybridCache hybridCache
) : IRequestHandler<IssueInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<IssueInvoiceCommandHandler> _logger = logger;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly HybridCache _hybridCache = hybridCache;

    public async Task<Result<InvoiceDto>> Handle(
        IssueInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var workOrder = await _context
            .WorkOrders.Include(w => w.Vehicle!)
                .ThenInclude(v => v.Customer)
            .Include(w => w.RepairTasks)
                .ThenInclude(rt => rt.Parts)
            .FirstOrDefaultAsync(w => w.Id == request.WorkOrderId, cancellationToken);

        if (workOrder is null)
        {
            _logger.LogWarning(
                "Invoice issuance failed. WorkOrder {WorkOrderId} not found.",
                request.WorkOrderId
            );

            return ApplicationErrors.WorkOrderNotFound;
        }

        if (workOrder.State != WorkOrderState.Completed)
        {
            _logger.LogWarning(
                "Invoice issuance rejected. WorkOrder {WorkOrderId} is not in completed.",
                request.WorkOrderId
            );

            return ApplicationErrors.WorkOrderMustBeCompletedForInvoicing;
        }

        var InvoiceId = Guid.NewGuid();

        int LineNumber = 1;

        var lineItems = new List<InvoiceLineItem>();
        foreach (var (task, taskIndex) in workOrder.RepairTasks.Select((r, i) => (r, i + 1)))
        {
            var partsSummary = task.Parts.Any()
                ? string.Join(
                    Environment.NewLine,
                    task.Parts.Select(p => $"    • {p.Name} x{p.Quantity} @ {p.Cost:C}")
                )
                : "    • No Parts";

            var lineDescription =
                $"{taskIndex}: {task.Name}{Environment.NewLine}"
                + $"  Labor = {task.LaborCost:C}{Environment.NewLine}"
                + $"  Parts: {Environment.NewLine}{partsSummary}";

            var totalTaskCost = task.TotalCost;
            var lineItemResult = InvoiceLineItem.Create(
                InvoiceId,
                lineNumber: LineNumber++,
                quantity: 1,
                unitPrice: totalTaskCost,
                description: lineDescription
            );

            if (lineItemResult.IsError)
                return lineItemResult.Errors!;
            lineItems.Add(lineItemResult.Value);
        }

        var subTotal = lineItems.Select(lt => lt.LineTotal);
        var discount = workOrder.Discount ?? 0m;

        var TaxRate = MechanicalShopConstants.TaxRate;

        var invoiceResult = Invoice.Create(
            InvoiceId,
            workOrder.Id,
            TaxRate,
            discount,
            lineItems,
            _timeProvider
        );

        if (invoiceResult.IsError)
        {
            _logger.LogWarning(
                "Invoice creation failed for WorkOrderId: {WorkOrderId}. Errors: {@Errors}",
                request.WorkOrderId,
                invoiceResult.Errors
            );
            return invoiceResult.Errors!;
        }

        var invoice = invoiceResult.Value;

        _context.Invoices.Add(invoice);
        await _hybridCache.RemoveAsync("invoice", cancellationToken);

        return invoice.ToDto();
    }
}
