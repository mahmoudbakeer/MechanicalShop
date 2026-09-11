using MechanicShop.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(rt => rt.Token).IsRequired();
        builder.Property(rt => rt.UserId).IsRequired();
        builder.Property(rt => rt.ExpiresAtUtc).IsRequired();
        builder.HasIndex(rt => rt.Token).IsUnique();
    }
}
