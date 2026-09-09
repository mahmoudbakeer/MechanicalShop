using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Common.Models;
using MechanicShop.Application.Features.Labors.LaborDtos;
using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MechanicShop.Application.Features.Labors.Queries;

public class GetEmployeesQueryHandler(
    IAppDbContext context,
    ILogger<GetEmployeesQueryHandler> logger
) : IRequestHandler<GetEmployeesQuery, Result<PaginatedList<LaborDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetEmployeesQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<LaborDto>>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Fetching Customers started, Page '{PageNumber}' PageSize '{PageSize}'",
            request.Page,
            request.PageSize
        );
        var employees = await _context
            .Employees.Where(e => e.Role == Role.Labor)
            .OrderBy(x => x.Id)
            .AsNoTracking()
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        var count = await _context.Employees.CountAsync(cancellationToken);
        return new PaginatedList<LaborDto>
        {
            Items = employees.ToDto(),
            TotalNumber = count,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(count / (double)request.PageSize),
        };
    }
}
