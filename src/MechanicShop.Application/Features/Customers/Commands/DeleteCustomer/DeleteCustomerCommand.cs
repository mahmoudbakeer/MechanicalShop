using MechanicShop.Domain.Common.Results;
using MediatR;

namespace MechanicShop.Application.Features.Customers.Commands.DeleteCustomer;

public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Result<Deleted>>;
