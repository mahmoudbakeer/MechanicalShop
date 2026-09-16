using System.IdentityModel.Tokens.Jwt;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Employees.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Infrastructure.Identity.Policies;

public class AssignLaborRequirement : IAuthorizationRequirement;

public class AssignLaborHandler(IAppDbContext context, IHttpContextAccessor contextAccessor) // the IhttpContextAccessor is used to access the WorkOrderId
    : AuthorizationHandler<AssignLaborRequirement>
{
    private readonly IAppDbContext _context = context;
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AssignLaborRequirement requirement
    )
    {
        var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(userId, out var employeeId))
        {
            context.Fail();
            return;
        }

        if (context.User.IsInRole(nameof(Role.Manager)))
        {
            context.Succeed(requirement);
            return;
        }
        // extract the work order id from the route exist in the request
        var routeValueId = _contextAccessor
            .HttpContext?.Request?.RouteValues["WorkOrderId"]
            ?.ToString();

        if (!Guid.TryParse(routeValueId, out Guid workOrderId))
        {
            context.Fail();
            return;
        }

        var isAssigned = await _context.WorkOrders.AnyAsync(wo =>
            wo.Id == workOrderId && wo.EmployeeId == employeeId
        );

        if (isAssigned)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
        return;
    }
}
