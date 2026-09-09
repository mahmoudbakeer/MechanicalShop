using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;

public class RelocateWorkOrderValidator : AbstractValidator<RelocateWorkOrderCommand>
{
    public RelocateWorkOrderValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Work order ID is required.");
        RuleFor(x => x.StartAt).NotEmpty().WithMessage("Start time is required.");
        RuleFor(x => x.Spot).NotNull().WithMessage("Spot is required.");
    }
}
