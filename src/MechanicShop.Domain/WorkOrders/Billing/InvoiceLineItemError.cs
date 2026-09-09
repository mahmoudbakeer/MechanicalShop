using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;

public static class InvoiceLineItemError
{
    public static Error InvoiceIdRequired =>
        Error.Validation("Invoice_Id_Required.", "Invoice Id cannot be null or empty.");

    public static Error DescriptionRequired =>
        Error.Validation(
            "InvoiceLineItem_Description_Required.",
            "Description cannot be null or empty."
        );
    public static Error QuantityInvalid =>
        Error.Validation(
            "InvoiceLineItem_Quantity_Invalid.",
            "Quantity Invalid must be between 1 and 10."
        );
    public static Error LineNumberInvalid =>
        Error.Validation("InvoiceLineItem_LineNumber_Invalid.", "LineNumber Invalid.");
    public static Error UnitPriceInValid =>
        Error.Validation(
            "InvoiceLineItem_UnitPrice_Invalid.",
            "UnitPrice Invalid must be between 1$ - 10,000$."
        );
}

