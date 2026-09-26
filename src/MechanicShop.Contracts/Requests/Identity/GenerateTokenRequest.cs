using System.ComponentModel.DataAnnotations;

namespace MechanicShop.Contracts.Requests.Identity;

public class GenerateTokenRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email address not valid.")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = default!;
}
