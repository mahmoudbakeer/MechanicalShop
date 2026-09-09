using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;

public sealed record GetInvoicePdfQuery(Guid InvoiceId) : ICachedQuery<Result<InvoicePdfDto>>
{
    public string CacheKey => $"InvoicePdf_{InvoiceId}";

    public string[] Tage => ["InvoicePdf"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
