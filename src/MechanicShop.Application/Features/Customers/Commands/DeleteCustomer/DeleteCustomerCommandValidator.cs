using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(t => t.Id).NotEmpty().WithMessage("Customer Id required, cannot be null or empty.");
    }
}
