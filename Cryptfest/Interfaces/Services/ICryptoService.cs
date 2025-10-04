using API.Data.Entities.UserEntities;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface ICryptoService
{
    Task<ToClientDto> GetAssetBySymbolAsync(string symbol);
    Task<ToClientDto> GetWalletAsync(int walletId);
    Task<bool> CreateWallet(API.Data.Entities.UserEntities.User user);
    Task<ToClientDto> EnsureDepositAsync(int walletId, decimal amount);

}
