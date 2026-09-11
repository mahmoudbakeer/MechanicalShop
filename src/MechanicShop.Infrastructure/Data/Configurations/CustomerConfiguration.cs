using MechanicShop.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Email).HasMaxLength(100);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(20);
        builder
            .HasMany(c => c.Vehicles)
            .WithOne(v => v.Customer)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Vehicles).UsePropertyAccessMode(PropertyAccessMode.Field);
        // what does this line do? It tells EF Core to use the field instead of the property for the navigation property Vehicles.
        // means use _vehicles instead of Vehicles as backing field, you can use .HasField("_vehicles") to specify the field name explicitly. after Navigation()
        // This is useful when you want to encapsulate the collection and prevent external code from modifying it directly.
    }
}
