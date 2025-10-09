using API.Data.Entities.UserEntities;
using API.Data.Entities.WalletEntities;
using Cryptfest.Model.Dtos;


namespace Cryptfest.Interfaces.Services;

public interface ICryptoService
{
    Task<ToClientDto> GetAssetsAsync(Guid walletId);
    Task<ToClientDto> GetAssetBySymbolAsync(string symbol);
    Task<ToClientDto> GetWalletAsync(Guid walletId);
    Task<Wallet> CreateWallet(User user);
    Task<ToClientDto> EnsureDepositAsync(Guid walletId, decimal amount);
    Task<ToClientDto> EnsureExchangeAsync(Guid walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount);
    Task<ToClientDto> GetWalletBalancesAsync(Guid walletId);
    Task<ToClientDto> GetWalletStatisticAsync(Guid walletId);
    Task<ToClientDto> GetWalletTransaction(Guid walletId);
}
