using API.Data.Entities.UserEntities;
using API.Data.Entities.WalletEntities;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface ICryptoService
{
    Task<ToClientDto> GetAssetsAsync();
    Task<ToClientDto> GetAssetBySymbolAsync(string symbol);
    Task<ToClientDto> GetWalletAsync(int walletId);
    Task<Wallet> CreateWallet(User user);
    Task<ToClientDto> EnsureDepositAsync(int walletId, decimal amount);
    Task<ToClientDto> EnsureExchangeAsync(int walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount);
    Task<ToClientDto> GetWalletBalancesAsync(int walletId);
    Task<ToClientDto> GetWalletStatisticAsync(int walletId);
    Task<ToClientDto> GetWalletTransaction(int walletId);
}
