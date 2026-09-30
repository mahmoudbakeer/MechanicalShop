using Asp.Versioning;
using MechanicShop.Application.Features.Billing.BillingDtos;
using MechanicShop.Application.Features.Billing.Commands.IssueInvoice;
using MechanicShop.Application.Features.Billing.Commands.SettleInvoice;
using MechanicShop.Application.Features.Billing.Queries.GetInvoiceById;
using MechanicShop.Application.Features.Billing.Queries.GetInvoicePdf;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MechanicShop.Api.Controllers;

[Route("api/v{version:apiVersion}/invoices")]
[Authorize(Policy = "ManagerOnly")]
public class InvoiceController(ISender sender) : ControllerApi
{
    [HttpPost("workorders/{workOrderId:Guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [EndpointSummary("Issue Invoice.")]
    [EndpointDescription("Issue an Invoice for a specific workOrder if it's allowed.")]
    [EndpointName("IssueInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<InvoiceDto>> IssueInvoice(
        [FromRoute] Guid workOrderId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new IssueInvoiceCommand(workOrderId), ct);
        return result.Match(
            i =>
                CreatedAtAction(
                    actionName: nameof(GetInvoiceById),
                    routeValues: new { invoiceId = i.Id },
                    value: i
                ),
            Problem!
        );
    }

    [HttpGet("{invoiceId:Guid}")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve an Invoice.")]
    [EndpointDescription("Return an Invoice and all it's details if exists.")]
    [EndpointName("GetInvoiceById")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(
        [FromRoute] Guid invoiceId,
        CancellationToken ct
    )
    {
        var result = await sender.Send(new GetInvoiceByIdQuery(invoiceId), ct);

        return result.Match(Ok, Problem!);
    }

    [HttpPut("{invoiceId:Guid}/payment")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [EndpointSummary("Pay an Invoice.")]
    [EndpointDescription("Settle an Invoice and mark it as paid if it's valid.")]
    [EndpointName("SettleInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> SettleInvoice([FromRoute] Guid invoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new SettleInvoiceCommand(invoiceId), ct);

        return result.Match(_ => NoContent(), Problem!);
    }

    [HttpGet("{invoiceId:Guid}/pdf")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionResult), StatusCodes.Status200OK)]
    [EndpointSummary("Retrieve an Invoice pdf file.")]
    [EndpointDescription("Return an Invoice pdf file containing all its details if valid.")]
    [EndpointName("GetInvoicePdf")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetInvoicePdf([FromRoute] Guid invoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInvoicePdfQuery(invoiceId), ct);

        return result.Match(rsp => File(rsp.Content!, "application/pdf", rsp.FileName), Problem!);
    }
}
