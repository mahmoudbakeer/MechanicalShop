using FluentValidation;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(1).WithMessage("Page number must be greater than 1.");
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 20)
            .WithMessage("Page size must be between 1 and 20.");
    }
}
