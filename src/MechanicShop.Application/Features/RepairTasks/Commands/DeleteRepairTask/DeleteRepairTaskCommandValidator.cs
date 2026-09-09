using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Commands.DeleteRepairTask;

public class DeleteRepairTaskCommandValidator : AbstractValidator<DeleteRepairTaskCommand>
{
    public DeleteRepairTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Repair task ID must not be empty.");
    }
}
