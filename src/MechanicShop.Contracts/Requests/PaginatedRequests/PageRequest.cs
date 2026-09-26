using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.PaginatedRequests;

public class PageRequest
{
    [Range(1, 100, ErrorMessage = "The Page must be at least 1.")]
    [Required(ErrorMessage = "Page is required.")]
    public int Page { get; set; } = 1;
    [Range(10, 100, ErrorMessage = "The Page must be at least 1.")]
    [Required(ErrorMessage = "PageSize is required.")]
    public int PageSize { get; set; } = 10;
}
