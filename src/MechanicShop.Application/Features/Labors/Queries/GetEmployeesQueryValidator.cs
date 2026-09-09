using FluentValidation;

namespace MechanicShop.Application.Features.Labors.Queries;

public sealed class GetEmployeesQueryValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(1).WithMessage("Page number must be greater than 1.");
        RuleFor(x => x.PageSize)
            .ExclusiveBetween(1, 20)
            .WithMessage("Page size must be greater than 1 and less thab 100.");
    }
}
