
using FluentValidation;
using FluentValidation.Validators;
using MechanicShop.Domain.RepairTasks;

namespace MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;

public sealed class CreateRepairTaskCommandValidator : AbstractValidator<CreateRepairTaskCommand>
{
    public CreateRepairTaskCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(RepairTaskError.NameRequired.Description)
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.LaborCost)
            .InclusiveBetween(1, 10000).WithMessage(RepairTaskError.LaborCostInValid.Description);

        RuleFor(x => x.Duration)
            .IsInEnum().WithMessage(RepairTaskError.DurationRequired.Description);

        RuleFor(x => x.Parts)
            .NotEmpty().WithMessage(RepairTaskError.PartsRequired.Description);

        RuleForEach(x => x.Parts).SetValidator(new CreatePartCommandValidator());
    }
}
