using MechanicShop.Application.Common.Errors;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MechanicShop.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler(
    ILogger<GetCustomerByIdQuery> logger,
    IAppDbContext context
) : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ILogger<GetCustomerByIdQuery> _logger = logger;
    private readonly IAppDbContext _context = context;

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var customer = await _context.Customers.FindAsync(request.Id, cancellationToken);

        if (customer is null)
        {
            _logger.LogWarning(
                "Customer not found, Customer with id '{CustomerId}' does not exist.",
                request.Id
            );
            return ApplicationErrors.CustomerNotFound;
        }

        return customer.ToDto();
    }
}
