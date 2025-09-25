using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface ICryptoService
{
    Task<ToClientDto> GetAssetBySymbolAsync(string symbol);
    Task<ToClientDto> GetWalletAsync(int walletId);
}
