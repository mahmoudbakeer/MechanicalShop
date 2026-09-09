using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(t => t.Name).NotEmpty().MaximumLength(50).WithMessage("Name cannot be null or empty.");
        RuleFor(t => t.Email).EmailAddress().WithMessage("Invalid Email Address.").MaximumLength(50);
        RuleFor(t => t.PhoneNumber).NotEmpty().Matches(@"^\+[1-9]\d{7,14}$").WithMessage("Invalid PhoneNumber format.").MaximumLength(50);
        RuleForEach(t => t.Vehicles).SetValidator(new CreateVehicleCommandValidator());
    }
}
