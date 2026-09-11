using MechanicShop.Domain.RepairTasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations;

public class RepairTaskConfiguration : IEntityTypeConfiguration<RepairTask>
{
    public void Configure(EntityTypeBuilder<RepairTask> builder)
    {
        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(rt => rt.LaborCost).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(rt => rt.EstimatedDuration).HasConversion<string>().IsRequired();
        builder.Ignore(rt => rt.TotalCost);
        builder
            .HasMany(c => c.Parts)
            .WithOne()
            .HasForeignKey("RepairTaskId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Parts).UsePropertyAccessMode(PropertyAccessMode.Field);

        // what does this line do? It tells EF Core to use the field instead of the property for the navigation property Vehicles.
        // means use _vehicles instead of Vehicles as backing field, you can use .HasField("_vehicles") to specify the field name explicitly. after Navigation()
        // This is useful when you want to encapsulate the collection and prevent external code from modifying it directly.
    }
}
