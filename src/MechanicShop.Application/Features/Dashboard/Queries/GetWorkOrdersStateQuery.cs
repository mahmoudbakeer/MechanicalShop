using MechanicShop.Application.Features.Dashboard.DashboardDtos;
using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Dashboard.Queries;

public sealed record GetWorkOrdersStateQuery(DateOnly Date) : IRequest<Result<WorkOrdersState>>;
