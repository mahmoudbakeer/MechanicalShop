using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public class InvoiceLineItem : AuditableEntity
{
    public int LineNumber { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Description { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;
#pragma warning disable CS8618
    private InvoiceLineItem() { }
#pragma warning disable CS8618

    private InvoiceLineItem(Guid id, int lineNumber, int quantity, decimal unitPrice, string description) : base(id)
    {
        LineNumber = lineNumber;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Description = description;
    }

    public static Result<InvoiceLineItem> Create(Guid id, int lineNumber, int quantity, decimal unitPrice, string description)
    {
        if (Guid.Empty == id) return InvoiceLineItemError.InvoiceLineItemIdRequired;
        if (lineNumber < 1) return InvoiceLineItemError.LineNumberInvalid;
        if (quantity < 1 || quantity > 10) return InvoiceLineItemError.QuantityInvalid;
        if (unitPrice < 1 || unitPrice > 10000) return InvoiceLineItemError.UnitPriceInValid;
        if (string.IsNullOrEmpty(description)) return InvoiceLineItemError.DescriptionRequired;



        return new InvoiceLineItem(id, lineNumber, quantity, unitPrice, description);
    }

    public Result<Updated> Update(int lineNumber, int quantity, decimal unitPrice, string description)
    {
        if (lineNumber < 1) return InvoiceLineItemError.LineNumberInvalid;
        if (quantity < 1 || quantity > 10) return InvoiceLineItemError.QuantityInvalid;
        if (unitPrice < 1 || unitPrice > 10000) return InvoiceLineItemError.UnitPriceInValid;
        if (string.IsNullOrEmpty(description)) return InvoiceLineItemError.DescriptionRequired;

        LineNumber = lineNumber;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Description = description;
        return Result.Updated;
    }
}