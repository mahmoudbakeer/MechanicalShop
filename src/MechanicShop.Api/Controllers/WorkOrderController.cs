using Asp.Versioning;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Scheduling.Queries;
using MechanicShop.Application.Features.Scheduling.SchedulingDtos;
using MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;
using MechanicShop.Application.Features.WorkOrders.Commands.CreateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Commands.DeleteWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Commands.RelocateWorkOrder;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateworkOrderRepairTasks;
using MechanicShop.Application.Features.WorkOrders.Commands.UpdateWorkOrderState;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrderById;
using MechanicShop.Application.Features.WorkOrders.Queries.GetWorkOrders;
using MechanicShop.Application.Features.WorkOrders.WorkOrderDtos;
using MechanicShop.Contracts.Requests.PaginatedRequests;
using MechanicShop.Contracts.Requests.WorkOrders;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/workorders")]
[Authorize]
public class WorkOrderController(ISender sender) : ControllerApi
{
    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginatedList<WorkOrderListItemDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve a list WorkOrders.")]
    [EndpointDescription(
        "Returns list of WorkOrders and their details if exists with specific page and pageSize."
    )]
    [EndpointName("GetWorkOrders")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<PaginatedList<WorkOrderListItemDto>>> GetWorkOrders(
        [FromQuery] PageRequest pageRequest,
        [FromQuery] WorkOrdersFilter filter
    )
    {
        var result = await sender.Send(
            new GetWorkOrdersQuery(
                pageRequest.Page,
                pageRequest.PageSize,
                filter.SearchTerm,
                filter.SortColumn,
                filter.SortDirection,
                filter.State is not null ? (WorkOrderState)(int)filter.State : null,
                filter.VehicleId,
                filter.LaborId,
                filter.StartDateFrom,
                filter.StartDateTo,
                filter.EndDateFrom,
                filter.EndDateTo,
                filter.Spot is not null ? (Spot)(int)filter.Spot : null
            )
        );

        return result.Match(Ok, Problem!);
    }

    [HttpGet("{workOrderId:Guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Retrieve a workOrder by Id.")]
    [EndpointDescription("Returns full details about specific order if it's exists.")]
    [EndpointName("GetWorkOrderById")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<WorkOrderDto>> GetWorkOrderById([FromRoute] Guid workOrderId)
    {
        var result = await sender.Send(new GetWorkOrderByIdQuery(workOrderId));
        return result.Match(Ok, Problem!);
    }

    [HttpPut("{workOrderId:Guid}/labor")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Assign Labor to WorkOrder.")]
    [EndpointDescription("Assign a Labor to WorkOrder if it's available.")]
    [EndpointName("AssignLabor")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AssignLabor(
        [FromRoute] Guid workOrderId,
        [FromBody] AssignLaborRequest request
    )
    {
        var result = await sender.Send(new AssignLaborCommand(workOrderId, request.LaborId));

        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpPut("{workOrderId:Guid}/relocation")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Relocate the WorkOrder to another bay.")]
    [EndpointDescription("Relocate the WorkOrder to another bay if it's available.")]
    [EndpointName("RelocateWorkOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RelocateWorkOrder(
        [FromRoute] Guid workOrderId,
        [FromBody] RelocateWorkOrderRequest request
    )
    {
        var result = await sender.Send(
            new RelocateWorkOrderCommand(workOrderId, request.StartAt, (Spot)(int)request.Spot)
        );

        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpPut("{workOrderId:Guid}/spot")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointSummary("Update WorkOrder RepairTasks.")]
    [EndpointDescription("Update WorkOrder RepairTask by their Ids.")]
    [EndpointName("UpdateWorkOrderRepairTasks")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateWorkOrderRepairTasks(
        [FromRoute] Guid workOrderId,
        [FromBody] UpdateRepairTasksRequest request
    )
    {
        var result = await sender.Send(
            new UpdateWorkOrderRepairTasksCommand(workOrderId, request.RepairTasks)
        );

        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpDelete("{workOrderId:Guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointSummary("Delete WorkOrder.")]
    [EndpointName("DeleteWorkOrder")]
    [EndpointDescription("Delete WorkOrder by Id if allowed.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> DeleteWorkOrder([FromRoute] Guid workOrderId)
    {
        var result = await sender.Send(new DeleteWorkOrderCommand(workOrderId));
        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpPut("{workOrderId:Guid}/state")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointName("UpdateWorkOrderState")]
    [EndpointSummary("Update WorkOrder State.")]
    [EndpointDescription("Update WorkOrder State if allowed.")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateWorkOrderState(
        [FromRoute] Guid workOrderId,
        [FromBody] UpdateWorkOrderStateRequest request
    )
    {
        var result = await sender.Send(
            new UpdateWorkOrderStateCommand(workOrderId, (WorkOrderState)(int)request.NewState)
        );
        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpPost]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(WorkOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [EndpointName("Create")]
    [EndpointSummary("Create WorkOrder.")]
    [EndpointDescription("Create a WorkOrder and return it's Id.")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<WorkOrderDto>> Create(
        [FromBody] CreateWorkOrderRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new CreateWorkOrderCommand(
                request.LaborId,
                request.VehicleId,
                request.StartAt,
                (Spot)(int)request.Spot,
                request.RepairTaskIds
            ),
            ct
        );
        return result.Match(
            Wo =>
                CreatedAtAction(
                    nameof(GetWorkOrderById),
                    routeValues: new { workOrderId = Wo.Id },
                    value: Wo
                ),
            Problem!
        );
    }

    [HttpGet("schedule/{date?}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [EndpointName("GetSchedule")]
    [EndpointSummary("Get Daily Schedule.")]
    [EndpointDescription("Retrieve the workOrder schedule of the specified day.")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ScheduleDto>> GetSchedule(
        DateOnly? date,
        [FromQuery] Guid? laborId,
        [FromHeader(Name = "X-TimeZone")] string? tz,
        CancellationToken ct
    )
    {
        if (string.IsNullOrWhiteSpace(tz))
            return Problem(
                detail: $"Timezone info header 'X-TimeZone' is missing.",
                title: "TimeZone required.",
                statusCode: StatusCodes.Status400BadRequest
            );
        TimeZoneInfo timeZone;
        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(tz);
        }
        catch
        {
            return Problem(
                detail: $"Timezone info header 'X-TimeZone' is invalid.",
                title: "TimeZone invalid.",
                statusCode: StatusCodes.Status400BadRequest
            );
        }
        var ScheduleDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var result = await sender.Send(
            new GetDailyScheduleQuery(
                timeZone,
                ScheduleDate,
                laborId == Guid.Empty ? null : laborId
            ),
            ct
        );

        return result.Match(Ok, Problem!);
    }
}
