using FluentValidation;

namespace MechanicShop.Application.Features.Dashboard.Queries;

public class GetWorkOrdersStateQueryValidator : AbstractValidator<GetWorkOrdersStateQuery>
{
    public GetWorkOrdersStateQueryValidator()
    {
        RuleFor(x => x.Date).NotEmpty().WithMessage("Date cannot be empty.");
    }
}
