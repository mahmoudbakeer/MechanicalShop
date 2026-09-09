using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;

public sealed class GetInvoiceByIdQueryHandler(
    IAppDbContext context,
    ILogger<GetInvoiceByIdQueryHandler> logger
) : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetInvoiceByIdQueryHandler> _logger = logger;

    public async Task<Result<InvoiceDto>> Handle(
        GetInvoiceByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning(
                "Invoice retrieval failed. Invoice with ID {InvoiceId} not found.",
                request.InvoiceId
            );
            return ApplicationErrors.InvoiceNotFound;
        }
        var invoiceDto = invoice.ToDto();
        return invoiceDto;
    }
}
