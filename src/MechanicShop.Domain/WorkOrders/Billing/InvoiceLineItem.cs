using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public class InvoiceLineItem
{
    public Guid InvoiceId { get; }
    public int LineNumber { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;
#pragma warning disable CS8618
    private InvoiceLineItem() { }
#pragma warning disable CS8618

    private InvoiceLineItem(
        Guid invoiceId,
        int lineNumber,
        int quantity,
        decimal unitPrice,
        string description
    )
    {
        InvoiceId = invoiceId;
        LineNumber = lineNumber;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Description = description;
    }

#pragma warning disable CS8618
    public static Result<InvoiceLineItem> Create(
        Guid InvoiceId,
        int lineNumber,
        int quantity,
        decimal unitPrice,
        string description
    )
    {
        if (Guid.Empty == InvoiceId)
            return InvoiceLineItemError.InvoiceIdRequired;
        if (lineNumber < 1)
            return InvoiceLineItemError.LineNumberInvalid;
        if (quantity < 1 || quantity > 10)
            return InvoiceLineItemError.QuantityInvalid;
        if (unitPrice < 1 || unitPrice > 10000)
            return InvoiceLineItemError.UnitPriceInValid;
        if (string.IsNullOrEmpty(description))
            return InvoiceLineItemError.DescriptionRequired;

        return new InvoiceLineItem(InvoiceId, lineNumber, quantity, unitPrice, description);
    }
}
