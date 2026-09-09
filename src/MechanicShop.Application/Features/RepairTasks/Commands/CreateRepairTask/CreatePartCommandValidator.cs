
using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed class CreatePartCommandValidator : AbstractValidator<CreatePartCommand>
{
    public CreatePartCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Part name is required.")
            .MaximumLength(100).WithMessage("Part name must not exceed 100 characters.");

        RuleFor(x => x.Cost)
            .InclusiveBetween(1, 10000).WithMessage("Part cost must be between 1 and 10,000.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 100).WithMessage("Part quantity must be between 1 and 100.");
    }
}