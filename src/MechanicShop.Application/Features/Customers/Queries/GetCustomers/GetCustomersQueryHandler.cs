using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Commands.Customers.CustomerDtos;
using MechanicShop.Application.Features.Customers.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler(
    IAppDbContext context,
    ILogger<GetCustomersQueryHandler> logger
) : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetCustomersQueryHandler> _logger = logger;

    public async Task<PaginatedList<CustomerDto>> Handle(
        GetCustomersQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Fetching Customers started, Page '{PageNumber}' PageSize '{PageSize}'",
            request.Page,
            request.PageSize
        );
        var customers = await _context
            .Customers.Include(c => c.Vehicles)
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var customersDto = customers.ToDto().ToList();
        var count = await _context.Customers.CountAsync(cancellationToken);

        var results = new PaginatedList<CustomerDto>
        {
            Items = customersDto,
            PageSize = request.PageSize,
            PageNumber = request.Page,
            TotalNumber = count,
            TotalPages = (int)Math.Ceiling(count / (double)request.PageSize),
        };

        return results;
    }
}
