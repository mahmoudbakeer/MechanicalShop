using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Application.Features.Billing.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var invoice = await _context.Invoices.Include(i => i.WorkOrder).ThenInclude(wo => wo.Vehicle).ThenInclude(v => v.Customer).FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning(
                "Invoice retrieval failed. Invoice with ID {InvoiceId} not found.",
                request.InvoiceId
            );
            return ApplicationErrors.InvoiceNotFound;
        }
        _logger.LogInformation(
    "Invoice {InvoiceId}: Status={Status}",
    invoice.Id,
    invoice.Status);
        var invoiceDto = invoice.ToDto();
        return invoiceDto;
    }
}
