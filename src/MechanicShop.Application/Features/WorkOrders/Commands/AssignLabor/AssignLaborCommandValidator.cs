using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;

public sealed class AssignLaborCommandValidator : AbstractValidator<AssignLaborCommand>
{
    public AssignLaborCommandValidator()
    {
        RuleFor(x => x.LaborId)
            .NotEmpty()
            .WithMessage("Labor Id is required, cannot be null or empty.");
        RuleFor(x => x.WorkOrderId)
            .NotEmpty()
            .WithMessage("WorkOrder Id is required, cannot be null or empty.");
    }
}
