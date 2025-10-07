using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.WalletEntities;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Repositories;

public interface ICryptoAssetRepository
{
    Task<List<CryptoAsset>> GetCryptoAssetsAsync();
    Task<CryptoAsset?> GetCryptoAssetBySymbolAsync(string symbol);
    Task<Wallet?> GetWalletByIdAsync(int id);
    Task<bool> SaveChangesAsync();
    public void Update(object entity);
    Task AddWalletAsync(Wallet wallet);
}
