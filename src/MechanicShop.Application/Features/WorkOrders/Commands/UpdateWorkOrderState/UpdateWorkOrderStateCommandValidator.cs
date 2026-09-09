using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;

public class UpdateWorkOrderStateCommandValidator : AbstractValidator<UpdateWorkOrderStateCommand>
{
    public UpdateWorkOrderStateCommandValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Work order ID is required.");
        RuleFor(x => x.NewState)
            .IsInEnum()
            .WithMessage("New state must be a valid WorkOrderState.");
    }
}
