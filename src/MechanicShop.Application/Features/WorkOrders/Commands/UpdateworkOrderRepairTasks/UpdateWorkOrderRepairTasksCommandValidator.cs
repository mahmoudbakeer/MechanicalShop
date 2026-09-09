using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateworkOrderRepairTasks;

public class UpdateWorkOrderRepairTasksCommandValidator
    : AbstractValidator<UpdateWorkOrderRepairTasksCommand>
{
    public UpdateWorkOrderRepairTasksCommandValidator()
    {
        RuleFor(x => x.RepairTasks)
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("RepairTask IDs must be unique.");
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Work order ID is required.");
        RuleFor(x => x.RepairTasks)
            .NotEmpty()
            .WithMessage("At least one repair task must be provided.");
    }
}
