using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos.Vehicle;

namespace MechanicShop.Application.Features.Billing.BillingDtos;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid WorkOrderId { get; set; }
    public DateTimeOffset IssuedAtUtc { get; set; }
    public CustomerDto? Customer { get; set; }
    public VehicleDto? Vehicle { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? PaymentStatus { get; set; }

    public List<InvoiceLineItemDto> Items { get; set; } = [];
}
