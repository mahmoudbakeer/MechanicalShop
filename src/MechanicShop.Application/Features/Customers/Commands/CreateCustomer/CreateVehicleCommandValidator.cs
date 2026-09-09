using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.CreateCustomer;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(t => t.Make).NotEmpty().MaximumLength(50);
        RuleFor(t => t.Model).NotEmpty().MaximumLength(50);
        RuleFor(t => t.Year).NotEmpty().ExclusiveBetween(1860, DateTime.UtcNow.Year);
        RuleFor(t => t.LicensePlate).NotEmpty().MaximumLength(50);
    }
}
