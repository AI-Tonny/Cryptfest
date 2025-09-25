using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.Api;

namespace Cryptfest.Interfaces.Repositories;

public interface ICryptoAssetRepository
{
    Task<List<CryptoAsset>> GetCryptoAssetsAsync();
    ApiAccess GetApiAccess();
    Task<CryptoAsset?> GetCryptoAssetBySymbolAsync(string symbol);
    Task<Wallet?> GetWalletByIdAsync(int id);
    Task<bool> SaveChangesAsync();
}
