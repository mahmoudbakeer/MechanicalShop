using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Application.Features.Identity;
using MechanicShop.Application.Features.Identity.IdentityDtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MechanicShop.Infrastructure.Identity;

public class TokenProvider(
    IConfiguration configuration,
    IAppDbContext context,
    TimeProvider timeProvider
) : ITokenProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IAppDbContext _context = context;
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<Result<TokenResponse>> GenerateTokenAsync(
        AppUserDto user,
        CancellationToken ct
    )
    {
        var tokenResult = await CreateTokenAsync(user, ct);

        if (tokenResult.IsError)
        {
            return tokenResult.Errors!;
        }

        return tokenResult.Value;
    }

    private async Task<Result<TokenResponse>> CreateTokenAsync(
        AppUserDto user,
        CancellationToken ct
    )
    {
        var JwtSettings = _configuration.GetSection("JwtSettings");

        var issuer = JwtSettings["Issuer"];
        var audience = JwtSettings["Audience"];
        var key = JwtSettings["Secret"];
        var now = _timeProvider.GetUtcNow();
        var expires = now.AddMinutes(int.Parse(JwtSettings["TokenExperationInMinutes"]!));

        var claims = new List<Claim>()
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId),
            new(JwtRegisteredClaimNames.Email, user.Email!),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Expires = expires.UtcDateTime,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken securityToken = tokenHandler.CreateToken(descriptor);

        await _context.RefreshTokens.Where(rt => rt.UserId == user.UserId).ExecuteDeleteAsync(ct);

        var newRefreshToken = RefreshToken.Create(
            Guid.NewGuid(),
            GenerateRefreshToken(),
            user.UserId,
            now.UtcDateTime.AddDays(7)
        );

        if (newRefreshToken.IsError)
        {
            return newRefreshToken.Errors!;
        }
        var rt = newRefreshToken.Value;
        _context.RefreshTokens.Add(rt);
        await _context.SaveChangesAsync(ct);
        var tokenResult = new TokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = rt.Token,
            ExpiresOnUtc = expires.UtcDateTime,
        };

        return tokenResult;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public ClaimsPrincipal? GetPrincipalsFromExpiredToken(string expiredToken)
    {
        var JwtSettings = _configuration.GetSection("JwtSettings");
        var issuer = JwtSettings["Issuer"];
        var audience = JwtSettings["Audience"];
        var key = JwtSettings["Secret"];
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer,
            ValidateIssuer = true,
            ValidAudience = audience,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsPrincipal principal = tokenHandler.ValidateToken(
            expiredToken,
            validationParameters,
            out SecurityToken token
        );

        if (
            token is not JwtSecurityToken jwtSecurityToken
            || jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256Signature,
                StringComparison.InvariantCultureIgnoreCase
            )
        )
            throw new SecurityTokenException("Invalid token.");

        return principal;
    }
}
