using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.WorkOrders;
using MechanicShop.Domain.WorkOrders.Billing;
using Microsoft.EntityFrameworkCore;

namespace MechanicShop.Infrastructure.Data;



public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Part> Parts => Set<Part>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<RepairTask> RepairTasks => Set<RepairTask>();

    public DbSet<Invoice> Invoices => Set<Invoice>();

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
}