using FluentValidation;

namespace MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;

public sealed class GetRepairTasksQueryValidator : AbstractValidator<GetRepairTasksQuery>
{
    public GetRepairTasksQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(1).WithMessage("Page number must be greater than 1.");
        RuleFor(x => x.PageSize)
            .ExclusiveBetween(1, 20)
            .WithMessage("Page size must be greater than 1 and less thab 100.");
    }
}
