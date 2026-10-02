using Asp.Versioning;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using MechanicShop.Application.Features.RepairTasks.Commands.CreateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Commands.DeleteRepairTask;
using MechanicShop.Application.Features.RepairTasks.Commands.UpdateRepairTask;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;
using MechanicShop.Application.Features.RepairTasks.RepairTaskDtos;
using MechanicShop.Contracts.Requests.PaginatedRequests;
using MechanicShop.Contracts.Requests.RepairTasks;
using MechanicShop.Domain.RepairTasks.Enum;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/repair-tasks")]
[Authorize]
public class RepairTaskController(ISender sender) : ControllerApi
{


    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PaginatedList<RepairTaskDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve a list RepairTasks.")]
    [EndpointDescription(
        "Returns list of RepairTasks and their details if exists with specific page and pageSize."
    )]
    [EndpointName("GetRepairTasks")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<PaginatedList<RepairTaskDto>>> GetRepairTasks(
        [FromQuery] PageRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetRepairTasksQuery(request.Page, request.PageSize), ct);

        return result.Match(Ok, Problem!);
    }

    [HttpGet("{repairTaskId:Guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RepairTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve a repairTask by ID.")]
    [EndpointDescription("Returns full details about a specific repairTask if it exists.")]
    [EndpointName("GetRepairTaskById")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<RepairTaskDto>> GetRepairTaskById(
        [FromRoute] Guid repairTaskId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetRepairTaskByIdQuery(repairTaskId), ct);
        return result.Match(Ok, Problem!);
    }

    [HttpPut("{repairTaskId:Guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update RepairTask.")]
    [EndpointDescription("Update repair task if it is allowed.")]
    [EndpointName("UpdateRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateRepairTask(
        [FromRoute] Guid repairTaskId,
        [FromBody] UpdateRepairTaskRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new UpdateRepairTaskCommand(
                repairTaskId,
                request.Name,
                (RepairTaskDuration)(int)request.Duration,
                request.LaborCost,
                [
                    .. request.Parts.Select(p => new UpdatePartCommand(
                        p.Id,
                        p.Name,
                        p.Cost,
                        p.Quantity
                    )),
                ]
            ),
            ct
        );
        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(RepairTaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Create a RepairTask.")]
    [EndpointDescription("Create a repair task if it is valid.")]
    [EndpointName("CreateRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<RepairTaskDto>> CreateRepairTask(
        [FromBody] CreateRepairTaskRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new CreateRepairTaskCommand(
                request.Name,
                (RepairTaskDuration)(int)request.Duration,
                request.LaborCost,
                Parts:
                [
                    .. request.Parts.Select(p => new CreatePartCommand(p.Name, p.Cost, p.Quantity)),
                ]
            ),
            ct
        );

        return result.Match(
            rt =>
                CreatedAtAction(
                    nameof(GetRepairTaskById),
                    routeValues: new { repairTaskId = rt.Id },
                    value: rt
                ),
            Problem!
        );
    }

    [HttpDelete("{repairTaskId:Guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Delete a RepairTask.")]
    [EndpointDescription("Delete a repair task if it is allowed.")]
    [EndpointName("DeleteRepairTask")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> DeleteRepairTask(
        [FromRoute] Guid repairTaskId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new DeleteRepairTaskCommand(repairTaskId), ct);

        return result.Match(_ => NoContent(), Problem!);
    }
}
