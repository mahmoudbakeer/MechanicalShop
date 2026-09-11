using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MechanicShop.Infrastructure.Data.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.Property(wo => wo.Discount).IsRequired().HasPrecision(18, 2);
        builder.Property(wo => wo.StartedAtUtc).IsRequired();
        builder.Property(wo => wo.EndAtUtc).IsRequired();
        builder.Property(wo => wo.State).HasConversion<string>().IsRequired();
        builder.Property(wo => wo.Spot).HasConversion<string>().IsRequired();
        builder.Property(wo => wo.Discount).HasPrecision(18, 2).IsRequired();

        builder
            .HasMany(wo => wo.RepairTasks)
            .WithMany()
            .UsingEntity(j => j.ToTable("WorkOrderRepairTasks"));

        builder
            .HasOne(wo => wo.Invoice)
            .WithOne(i => i.WorkOrder)
            .HasForeignKey<Invoice>(i => i.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(wo => wo.TotalLaborCost);
        builder.Ignore(wo => wo.TotalPartsCost);
        builder.Ignore(wo => wo.Total);

        builder.HasOne(wo => wo.Employee).WithMany().HasForeignKey(wo => wo.EmployeeId);
        builder.HasOne(wo => wo.Vehicle).WithMany().HasForeignKey(wo => wo.VehicleId);
    }
}
