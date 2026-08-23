using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identity;

public static class RefreshTokenError
{
    public static Error RefreshTokenIdRequired =>
        Error.Validation("RefreshToken_Id_Required.", "RefreshTokenId cannot be null or empty.");
    public static Error UserIdReuired =>
        Error.Validation("RefreshToken_User_Id_Required.", "UserId cannot be null or empty.");
    public static Error TokenRequired =>
        Error.Validation("RefreshToken_Token_Required.", "Token cannot be null or empty.");
    public static Error InvalidExpiratioDate =>
        Error.Validation(
            "RefreshToken_ExpirationDate_Invalid.",
            "Invalid expiration date must be in the future."
        );
}
