namespace Cryptfest.Interfaces.Services.InitialCall;

public interface IInitialCallService
{
    Task<bool> SaveAssetsInDbFromApi();
    Task InitialApiAccess();
}
