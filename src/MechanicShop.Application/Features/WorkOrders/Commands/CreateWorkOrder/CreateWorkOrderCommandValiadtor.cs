using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.LaborId).NotEmpty().WithMessage("LaborId is required.");
        RuleFor(x => x.VehicleId).NotEmpty().WithMessage("VehicleId is required.");
        RuleFor(x => x.StartAt).NotEmpty().WithMessage("StartAt is required.");
        RuleFor(x => x.Spot).NotEmpty().WithMessage("Spot is required.");
        RuleFor(x => x.RepairTaskIds)
            .NotEmpty()
            .WithMessage("RepairTaskIds must contain at least one repair task ID.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("RepairTaskIds must not contain duplicate IDs.");
    }
}
