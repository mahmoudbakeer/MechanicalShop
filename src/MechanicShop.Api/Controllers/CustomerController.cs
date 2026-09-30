using Asp.Versioning;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Customers.Commands.CreateCustomer;
using MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;
using MechanicShop.Application.Features.Customers.Commands.UpdateCustomer;
using MechanicShop.Application.Features.Customers.Queries.GetCustomerById;
using MechanicShop.Application.Features.Customers.Queries.GetCustomers;
using MechanicShop.Contracts.Requests.Customers;
using MechanicShop.Contracts.Requests.PaginatedRequests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/customers")]
[Authorize]
public class CustomerController(ISender sender) : ControllerApi
{
    [HttpGet]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(PaginatedList<CustomerDto>), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve a list of Customers.")]
    [EndpointDescription(
        "Returns list of Customers and their details if exists with specific page and pageSize."
    )]
    [EndpointName("GetCustomers")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<PaginatedList<CustomerDto>>> GetCustomers(
        [FromQuery] PageRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetCustomersQuery(request.Page, request.PageSize), ct);

        return result.Match(Ok, Problem!);
    }

    [HttpGet("{customerId:Guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve a Customer by ID.")]
    [EndpointDescription("Returns full details about a specific Customer if it exists.")]
    [EndpointName("GetCustomerById")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        [FromRoute] Guid customerId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetCustomerByIdQuery(customerId), ct);
        return result.Match(Ok, Problem!);
    }

    [HttpPut("{customerId:Guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update Customer.")]
    [EndpointDescription("Update customer if it is allowed.")]
    [EndpointName("UpdateCustomer")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateCustomer(
        [FromRoute] Guid customerId,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new UpdateCustomerCommand(
                customerId,
                request.Name,
                request.Email,
                request.PhoneNumber,
                [
                    .. request.Vehicle.Select(v => new UpdateVehicleCommand(
                        v.Id,
                        v.Make,
                        v.Model,
                        v.Year,
                        v.LicensePlat
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
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Create a Customer.")]
    [EndpointDescription("Create customer if it is valid.")]
    [EndpointName("CreateCustomer")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerRequest request,
        CancellationToken ct
    )
    {
        var result = await sender.Send(
            new CreateCustomerCommand(
                request.Name,
                request.Email,
                request.PhoneNumber,
                [
                    .. request.Vehicle.Select(v => new CreateVehicleCommand(
                        v.Make,
                        v.Model,
                        v.Year,
                        v.LicensePlate
                    )),
                ]
            ),
            ct
        );

        return result.Match(
            rt =>
                CreatedAtAction(
                    nameof(GetCustomerById),
                    routeValues: new { customerId = rt.Id },
                    value: rt
                ),
            Problem!
        );
    }

    [HttpDelete("{customerId:Guid}")]
    [Authorize(Policy = "ManagerOnly")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Delete a Customer.")]
    [EndpointDescription("Delete a custoemr if it is allowed.")]
    [EndpointName("DeleteCustomer")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> DeleteCustomer(
        [FromRoute] Guid customerId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new DeleteCustomerCommand(customerId), ct);

        return result.Match(_ => NoContent(), Problem!);
    }
}
