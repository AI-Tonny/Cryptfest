using API.Data.Entities.Wallet;
using Cryptfest.Data.Entities.Api;

namespace Cryptfest.Interfaces.Repositories;

public interface ICryptoAssetRopository
{
    Task<List<CryptoAssetInfo>> GetCryptoAssetsAsync();
    ApiAccess GetApiAccess();
    Task<CryptoAssetInfo?> GetCryptoAssetBySymbolAsync(string symbol);
}
