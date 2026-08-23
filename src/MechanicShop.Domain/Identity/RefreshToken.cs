using MechanicShop.Domain.Common;
using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Identity;

public class RefreshToken : AuditableEntity
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
#pragma warning disable CS8618
    private RefreshToken() { }
#pragma warning disable CS8618

    private RefreshToken(Guid id, string token, string userId, DateTime expiresAtUtc)
        : base(id)
    {
        Token = token;
        UserId = userId;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static Result<RefreshToken> Create(
        Guid id,
        string token,
        string userId,
        DateTime expiresAtUtc
    )
    {
        if (Guid.Empty == id)
            return RefreshTokenError.RefreshTokenIdRequired;
        if (string.IsNullOrEmpty(userId))
            return RefreshTokenError.UserIdReuired;
        if (string.IsNullOrEmpty(token))
            return RefreshTokenError.TokenRequired;
        if (expiresAtUtc < DateTime.UtcNow)
            return RefreshTokenError.InvalidExpiratioDate;

        return new RefreshToken(id, token, userId, expiresAtUtc);
        ;
    }
}
