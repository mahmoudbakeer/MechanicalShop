using MechanicShop.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.Property(c => c.IssuedAtUtc).IsRequired();
        builder.Property(i => i.Status).HasConversion<string>().IsRequired();
        builder.Property(i => i.DiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(i => i.TaxAmount).IsRequired().HasColumnType("decimal(18,2)");

        builder.Ignore(i => i.Total);
        builder.Navigation(c => c.LineItems).UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.OwnsMany(
            c => c.LineItems,
            li =>
            {
                li.HasKey(li => new { li.InvoiceId, li.LineNumber });
                li.WithOwner().HasForeignKey(li => li.InvoiceId);
                li.Property(l => l.Description).IsRequired().HasMaxLength(100);
                li.Property(l => l.Quantity).IsRequired();
                li.Property(l => l.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
                li.Property(l => l.LineNumber).IsRequired();
            }
        );
    }
}
