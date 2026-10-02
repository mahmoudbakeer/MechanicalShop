using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice;

public sealed class SettleInvoiceCommandHandler(
    ILogger<SettleInvoiceCommandHandler> logger,
    IAppDbContext context,
    TimeProvider timeProvider,
    HybridCache HybridCache
) : IRequestHandler<SettleInvoiceCommand, Result<Success>>
{
    private readonly ILogger<SettleInvoiceCommandHandler> _logger = logger;
    private readonly IAppDbContext _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly HybridCache _hybridCache = HybridCache;

    public async Task<Result<Success>> Handle(
        SettleInvoiceCommand request,
        CancellationToken cancellationToken
    )
    {
        var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning(
                "Invoice Payment failed, Invoice with ID {InvoiceId} not found.",
                request.InvoiceId
            );
            return ApplicationErrors.InvoiceNotFound;
        }
        var payResult = invoice.PayInvoice(_timeProvider);
        if (payResult.IsError)
        {
            _logger.LogWarning(
                "Invoice payment failed for InvoiceId: {InvoiceId}. Errors: {Errors}",
                invoice.Id,
                payResult.Errors
            );

            return payResult.Errors!;
        }
        await _context.SaveChangesAsync(cancellationToken);

        await _hybridCache.RemoveAsync("invoice", cancellationToken);
        return Result.Success;
    }
}
