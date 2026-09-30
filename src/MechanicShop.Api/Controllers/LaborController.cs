using Asp.Versioning;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Application.Features.Labors.Queries;
using MechanicShop.Contracts.Requests.PaginatedRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/labors")]
[Authorize]
public class LaborController(ISender sender) : ControllerApi
{
    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PaginatedList<LaborDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve a list of Labors.")]
    [EndpointDescription(
        "Returns list of Employees and their details if exists with specific page and pageSize."
    )]
    [EndpointName("GetLabors")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<PaginatedList<LaborDto>>> GetLabors(
        [FromQuery] PageRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetEmployeesQuery(request.Page, request.PageSize), ct);

        return result.Match(Ok, Problem!);
    }
}
