using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(t => t.Id).NotEmpty().WithMessage("Customer Id required, cannot be null or empty.");
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Name cannot be null or empty.");
        RuleFor(t => t.Email)
            .EmailAddress()
            .WithMessage("Invalid Email Address.")
            .MaximumLength(50);
        RuleFor(t => t.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{7,14}$")
            .WithMessage("Invalid PhoneNumber format.")
            .MaximumLength(50);
        RuleForEach(t => t.Vehicles).SetValidator(new UpdateVehicleCommandValidator());
    }
}
