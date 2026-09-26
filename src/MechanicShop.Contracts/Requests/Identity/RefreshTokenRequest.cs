using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.Identity;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "RefreshToken is required.")]
    public string RefreshToken { get; set; } = default!;

    [Required(ErrorMessage = "AccessToken is required.")]
    public string ExpiredAccessToken { get; set; } = default!;
}
