using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;

public class GetInvoicePdfQueryHanlder(
    IAppDbContext context,
    ILogger<GetInvoicePdfQueryHanlder> logger,
    IInvoicePdfGenerator pdfGenerator
) : IRequestHandler<GetInvoicePdfQuery, Result<InvoicePdfDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetInvoicePdfQueryHanlder> _logger = logger;
    private readonly IInvoicePdfGenerator _pdfGenerator = pdfGenerator;

    public async Task<Result<InvoicePdfDto>> Handle(
        GetInvoicePdfQuery request,
        CancellationToken cancellationToken
    )
    {
        var invoice = await _context.Invoices.FindAsync([request.InvoiceId], cancellationToken);

        if (invoice is null)
        {
            _logger.LogInformation(
                "Invoice Pdf generation failed, Invoice with Id '{InvoiceId'} does not exist.",
                request.InvoiceId
            );

            return ApplicationErrors.InvoiceNotFound;
        }

        try
        {
            var pdfBytes = _pdfGenerator.Generate(invoice);

            var pdfDto = new InvoicePdfDto
            {
                Content = pdfBytes,
                FileName = $"Invoice_{request.InvoiceId}.pdf",
            };

            return pdfDto;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to generate PDF for Invoice with Id '{InvoiceId'}.",
                request.InvoiceId
            );
            return Error.Failure("An error occured while generating the pdf Invoice.");
        }
    }
}
