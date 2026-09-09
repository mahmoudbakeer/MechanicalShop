using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public class Invoice : AuditableEntity
{
    private readonly List<InvoiceLineItem> _lineItems = [];
    public IReadOnlyList<InvoiceLineItem> InvoiceLineItems => _lineItems; // same as the AsReadOnly()
    public WorkOrder WorkOrder { get; private set; }
    public Guid WorkOrderId { get; private set; }
    public DateTimeOffset PaidAtUtc { get; private set; }
    public DateTimeOffset IssuedAtUtc { get; private set; }

    public decimal SubTotal => _lineItems.Sum(li => li.LineTotal);
    public InvoiceStatus Status { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal Total => SubTotal - DiscountAmount + TaxAmount;
#pragma warning disable CS8618
    private Invoice() { }

#pragma warning disable CS8618

    private Invoice(
        Guid id,
        Guid workOrderId,
        DateTimeOffset issuedAt,
        decimal taxAmount,
        decimal discountAmount,
        List<InvoiceLineItem> lineItems
    )
        : base(id)
    {
        WorkOrderId = workOrderId;
        IssuedAtUtc = issuedAt;
        TaxAmount = taxAmount;
        DiscountAmount = discountAmount;
        _lineItems = lineItems;
        Status = InvoiceStatus.UnPaid;
    }

    public static Result<Invoice> Create(
        Guid id,
        Guid workOrderId,
        decimal taxAmount,
        decimal discountAmount,
        List<InvoiceLineItem> lineItems,
        TimeProvider timeProvider
    )
    {
        if (id == Guid.Empty)
            return InvoiceError.InvoiceIdRequired;
        if (workOrderId == Guid.Empty)
            return InvoiceError.WorkOrderIdRequired;
        if (!lineItems.Any())
            return InvoiceError.LinesItemEmpty;
        if (discountAmount < 0)
            return InvoiceError.DiscountNegative;
        if (discountAmount > lineItems.Sum(t => t.LineTotal))
            return InvoiceError.DiscountExceedSubtotal;
        return new Invoice(
            id,
            workOrderId,
            timeProvider.GetUtcNow(),
            taxAmount,
            discountAmount,
            [.. lineItems]
        );
    }

    public Result<Updated> ApplyDiscount(decimal discountAmount)
    {
        if (InvoiceStatus.UnPaid != Status)
            return InvoiceError.InvoiceLocked;
        if (discountAmount < 0)
            return InvoiceError.DiscountNegative;
        if (discountAmount > SubTotal)
            return InvoiceError.DiscountExceedSubtotal;
        DiscountAmount = discountAmount;
        return Result.Updated;
    }

    public Result<Updated> PayInvoice(TimeProvider timeProvider)
    {
        if (InvoiceStatus.UnPaid != Status)
            return InvoiceError.InvoiceLocked;

        Status = InvoiceStatus.Paid;
        PaidAtUtc = timeProvider.GetUtcNow();
        return Result.Updated;
    }
}
