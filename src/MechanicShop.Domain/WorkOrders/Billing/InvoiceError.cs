using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.WorkOrders.Billing;



public static class InvoiceError
{
    public static Error InvoiceIdRequired => Error.Validation("Invoice_Id_Required.", "Invoice Id cannot be null or empty.");
    public static Error WorkOrderIdRequired => Error.Validation("Invoice_WorkOrder_Id_Required.", "WorkOrder Id cannot be null or empty.");
    public static Error IssuedAtInvalid => Error.Validation("Invoice_IssuedAt_Invalid.", "IssuedAt Invalid must be today.");
    public static Error LinesItemEmpty => Error.Validation("Invoice_LinesItem_Invalid.", "LinesItems must contain at least one Item.");
    public static Error DiscountNegative => Error.Validation("Invoice_Discount_Negative.", "Discount cannot be negative.");
    public static Error DiscountExceedSubtotal => Error.Validation("Invoice_Discount_Exceed_SubTotal.", "Discount exceeded SubTotal.");
    public static Error InvoiceLocked => Error.Validation("Invoice_Locked.", "Invoice Locked that might be because it's already paid or refunded.");
}