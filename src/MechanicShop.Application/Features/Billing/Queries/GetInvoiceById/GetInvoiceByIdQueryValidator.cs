using FluentValidation;

namespace MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;

public sealed class GetInvoiceByIdQueryValidator : AbstractValidator<GetInvoiceByIdQuery>
{
    public GetInvoiceByIdQueryValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty().WithMessage("Invoice Id is required.");
    }
}
