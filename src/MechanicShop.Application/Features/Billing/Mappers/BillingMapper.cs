using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.WorkOrders.Billing;

namespace MechanicShop.Application.Features.Billing.Mappers;

public static class BillingMapper
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            WorkOrderId = invoice.WorkOrderId,
            Customer = invoice.WorkOrder!.Vehicle!.Customer!.ToDto(),
            Vehicle = invoice.WorkOrder.Vehicle.ToDto(),
            IssuedAtUtc = invoice.IssuedAtUtc,
            Subtotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            DiscountAmount = invoice.DiscountAmount,
            Total = invoice.Total,
            PaymentStatus = invoice.Status.ToString(),
            Items = [.. invoice.LineItems.Select(x => x.ToDto())],
        };
    }

    public static IList<InvoiceLineItemDto> ToDto(this IEnumerable<InvoiceLineItem> lineItems)
    {
        return [.. lineItems.Select(x => x.ToDto())];
    }

    public static IList<InvoiceDto> ToDto(this IEnumerable<Invoice> invoices)
    {
        return [.. invoices.Select(x => x.ToDto())];
    }

    public static InvoiceLineItemDto ToDto(this InvoiceLineItem lineItem)
    {
        return new InvoiceLineItemDto
        {
            InvoiceId = lineItem.InvoiceId,
            Description = lineItem.Description,
            Quantity = lineItem.Quantity,
            UnitPrice = lineItem.UnitPrice,
            LineTotal = lineItem.LineTotal,
        };
    }
}
