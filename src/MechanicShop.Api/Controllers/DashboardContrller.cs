using Asp.Versioning;
using MechanicShop.Application.Features.Dashboard.DashboardDtos;
using MechanicShop.Application.Features.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/dashboard")]
[Authorize]
public class DashboardController(ISender sender) : ControllerApi
{
    [HttpGet("state")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(WorkOrdersStates), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retreive the WorkOrders state.")]
    [EndpointDescription("Retreive the WorkOrders state in details for specified date.")]
    [EndpointName("GetDashboard")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<WorkOrdersStates>> GetDashboard(
        [FromQuery] DateOnly? date,
        CancellationToken ct
    )
    {
        var statsDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await sender.Send(new GetWorkOrdersStateQuery(statsDate), ct);

        return result.Match(Ok, Problem!);
    }
}
