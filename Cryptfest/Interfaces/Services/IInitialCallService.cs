namespace Cryptfest.Interfaces.Services;

public interface IInitialCallService
{
    Task<bool> SaveAssetsInDbFromApi();
}
