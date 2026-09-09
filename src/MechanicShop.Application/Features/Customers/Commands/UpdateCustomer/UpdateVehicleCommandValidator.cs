using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        RuleFor(t => t.Id).NotEmpty().WithMessage("Vehicle Id required, cannot be null or empty.");
        RuleFor(t => t.Make).NotEmpty().MaximumLength(50);
        RuleFor(t => t.Model).NotEmpty().MaximumLength(50);
        RuleFor(t => t.Year).NotEmpty().ExclusiveBetween(1860, DateTime.UtcNow.Year);
        RuleFor(t => t.LicensePlate).NotEmpty().MaximumLength(50);
    }
}
