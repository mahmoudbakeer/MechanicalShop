namespace MechanicShop.Application.Common.Models;

public sealed class PaginatedList<T>
{
    public int PageSize { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public int TotalNumber { get; init; }
    public IReadOnlyCollection<T>? Items { get; init; }
}

