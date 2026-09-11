namespace MechanicShop.Application.Common.Interfaces;

public interface IApplicationDbContextInitialiser
{
    Task InitialiseAsync();
    Task SeedAsync();
    Task TrySeedAsync();
}
