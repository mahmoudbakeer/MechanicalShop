using FluentValidation;

namespace MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById;

public class GetWorkOrderByIdQueryValidator : AbstractValidator<GetWorkOrderByIdQuery>
{
    public GetWorkOrderByIdQueryValidator()
    {
        RuleFor(x => x.WorkOrderId).NotEmpty().WithMessage("Work order ID must not be empty.");
    }
}
