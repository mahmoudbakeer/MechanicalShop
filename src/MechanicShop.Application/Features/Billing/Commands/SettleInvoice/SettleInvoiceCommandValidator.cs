using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Commands.SettleInvoice;

public sealed class SettleInvoiceCommandValidator : AbstractValidator<SettleInvoiceCommand>
{
    public SettleInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty().WithMessage("InvoiceId is required.");
    }
}
