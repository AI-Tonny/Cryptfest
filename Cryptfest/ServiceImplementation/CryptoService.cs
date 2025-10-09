using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using AutoMapper;
using Cryptfest.Data.Entities.ClientRequest;
using Cryptfest.Data.Entities.WalletEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System.Runtime.CompilerServices;

namespace Cryptfest.ServiceImpementation;

public class CryptoService : ICryptoService
{
    private readonly ICryptoAssetRepository _cryptoAssetRepository;
    private readonly IApiService _api;
    private readonly IMapper _mapper;

    public CryptoService(ICryptoAssetRepository cryptoAssetRepository, IApiService api, IMapper mapper)
    {
        _cryptoAssetRepository = cryptoAssetRepository;
        _api = api;
        _mapper = mapper;
    }


    private async Task UpdateWalletStatisticAsync(Wallet wallet)
    {
        var currentPrices = await _cryptoAssetRepository.GetCryptoAssetsAsync();

        decimal totalAssetsSum = 0;

        foreach (var balance in wallet.Balances)
        {
            var asset = currentPrices.FirstOrDefault(a => a.Symbol == balance.Asset.Symbol);
            if (asset is not null && asset.MarketData is not null)
            {
                totalAssetsSum += asset.MarketData.CurrPrice * balance.Amount;
            }
        }

        wallet.Statistic.TotalDeposit = wallet.Statistic.TotalDeposit;
        wallet.Statistic.TotalAssets = totalAssetsSum;
        wallet.Statistic.Apy = wallet.Statistic.TotalDeposit == 0
            ? 0
            : 100 * (totalAssetsSum - wallet.Statistic.TotalDeposit) / wallet.Statistic.TotalDeposit; // if(TotalDeposit == 0) { apy = 0; } else { apy =  100 * (totalAssetsSum ....)}


        await _cryptoAssetRepository.SaveChangesAsync();
    }


    public async Task<ToClientDto> GetAssetsAsync(Guid walletId)
    {
        return await _api.UpdateMarketDataInDbAsync(walletId);
    }

    public async Task<ToClientDto> GetAssetBySymbolAsync(string symbol)
    {
        CryptoAsset? output = await _cryptoAssetRepository.GetCryptoAssetBySymbolAsync(symbol);
        if(output is not null)
        {
            return new ToClientDto
            {
                Status = ResponseStatus.Success,
                Data = output
            };
        }
        else
        {
            return new ToClientDto
            {
                Status = ResponseStatus.Fail,
                Message = "This asset does not exist"
            };
        }
    }

    public async Task<ToClientDto> GetWalletAsync(Guid walletId)
    {

        List<CryptoAssetDto> currentPrices = (List<CryptoAssetDto>)(await _api.UpdateMarketDataInDbAsync(walletId)).Data!;
        Wallet? wallet = await _cryptoAssetRepository.GetWalletByIdAsync(walletId);


        if(currentPrices is null)
        {
            return new()
            {
                Message = "No asset data available or error in external API",
                Status = ResponseStatus.Fail,
            };
        }

        if (wallet is null)
        {
            return new()
            {
                Message = "Such wallet does not exist",
                Status = ResponseStatus.Fail,
            };
        }


        await UpdateWalletStatisticAsync(wallet);

        WalletDto walletResult = _mapper.Map<Wallet, WalletDto>(wallet);  

        ToClientDto output;
        bool saveResult = await _cryptoAssetRepository.SaveChangesAsync();

        output = new()
        {
            Status = ResponseStatus.Success,
            Data = walletResult,
        };

       return output;
    }

    public async Task<Wallet> CreateWallet(User user)
    {
        Wallet wallet = new()
        {
            User = user
        };
        await _cryptoAssetRepository.AddWalletAsync(wallet);
        await _cryptoAssetRepository.SaveChangesAsync();

        return wallet;
    }

    public async Task<ToClientDto> EnsureDepositAsync(Guid walletId, decimal amount)
    {
        await _api.UpdateMarketDataInDbAsync(walletId);

        Wallet? wallet = await _cryptoAssetRepository.GetWalletByIdAsync(walletId);
        CryptoAsset? assetUsdt = await _cryptoAssetRepository.GetCryptoAssetBySymbolAsync("usdt");

        if(assetUsdt is null)
        {
            return new()
            {
                Message = "No data in database",
                Status = ResponseStatus.Fail,
            };
        }

        else if (wallet is null)
        {
            return new()
            {
                Message = "Such wallet does not exist",
                Status = ResponseStatus.Fail,
            };
        }
        else if(amount < 0) 
        {
            return new()
            {
                Message = "The amount can not to be less than 0",
                Status = ResponseStatus.Fail,
            };
        }

        wallet.Statistic.TotalDeposit += amount;

        CryptoAsset? asset = await _cryptoAssetRepository.GetCryptoAssetBySymbolAsync("usdt");
        if (asset is null)
        {
            return new()
            {
                Status = ResponseStatus.Fail,
                Message = "This asset does not exist in database"
            };
        }

        CryptoBalance? usdt = wallet.Balances.FirstOrDefault(x => x.Asset.Symbol.ToUpper() == "USDT");


        if (usdt is not null)
        {
            usdt.Amount += amount;
            usdt.Usdt += amount;
        }
        else
        {

            wallet.Balances.Add(new CryptoBalance()
            {
                Amount = amount,
                Usdt = amount,
                Asset = asset,
                PurchasePrice = asset.MarketData.CurrPrice,
                Wallet = wallet
            });
        }

        wallet.Transactions.Add(new()
        {
            Amount = amount,
            ToAsset = assetUsdt,
            Date = DateTime.UtcNow,
            TransactionType = TransactionType.Deposit
        });

        bool saveResult = await _cryptoAssetRepository.SaveChangesAsync();

        await UpdateWalletStatisticAsync(wallet);

        WalletDto walletDto = _mapper.Map<Wallet, WalletDto>(wallet);

        if (saveResult == true)
        {
            return new()
            {
                Status = ResponseStatus.Success,
                Data = walletDto,
            };
        }
        else
        {
            return new()
            {
                Status = ResponseStatus.Fail,
                Message = "Failed to save to database"
            };
        }
    }

    public async Task<ToClientDto> EnsureExchangeAsync(Guid walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount)
    {
        await _api.UpdateMarketDataInDbAsync(walletId);
        Wallet? wallet = await _cryptoAssetRepository.GetWalletByIdAsync(walletId);
        if (wallet is null)
        {
            return new()
            {
                Message = "This wallet does not exist",
                Status = ResponseStatus.Fail,
            };
        }

        // does the fromAssetSymbol exist in wallet
        CryptoBalance? fromAsset = wallet.Balances.FirstOrDefault(x => x.Asset.Symbol.Equals(fromAssetSymbol,StringComparison.OrdinalIgnoreCase));
        
        

        // does the toAssetSymbol exist in db
        CryptoAsset? toAsset = await _cryptoAssetRepository.GetCryptoAssetBySymbolAsync(toAssetSymbol);
        if (fromAsset is null)
        {
            return new()
            {
                Message = "This asset does not exist in the user's wallet",
                Status = ResponseStatus.Fail,
            };
        }
        else if (fromAsset.Amount < amount || amount <= 0)
        {
            return new()
            {
                Message = "The wallet does not have the amount of this asset",
                Status = ResponseStatus.Fail,
            };
        }
        else if (toAsset is null)
        {
            return new()
            {
                Message = "The asset does not exist in db",
                Status = ResponseStatus.Fail,
            };
        }


        decimal newAssAmount = (amount * fromAsset.Asset.MarketData.CurrPrice) / toAsset.MarketData.CurrPrice;
        decimal purchasePrice = toAsset.MarketData.CurrPrice;

        // does this asset already exist in wallet if not, then create a new object of the instance, if yes, then update the info
        CryptoBalance? asset = wallet.Balances.FirstOrDefault(x => x.Asset.Symbol.Equals(toAssetSymbol,StringComparison.OrdinalIgnoreCase));
        if (asset is not null)
        {
            decimal totalOldValue = asset.PurchasePrice * asset.Amount;
            decimal totalNewValue = purchasePrice * newAssAmount;
            decimal totalAmount = asset.Amount + newAssAmount;

            asset.PurchasePrice = (totalOldValue + totalNewValue) / totalAmount; // we are looking for the average purchase price if the asset already existed
            asset.Amount = totalAmount;
            asset.Usdt = totalAmount * asset.PurchasePrice;
        }
        else
        {

            wallet.Balances.Add(new CryptoBalance()
            {
                Amount = newAssAmount,
                Usdt = purchasePrice * newAssAmount,
                Asset = toAsset,
                PurchasePrice = purchasePrice,
                Wallet = wallet
            });
        }

        fromAsset.Amount -= amount;
        fromAsset.Usdt = fromAsset.Amount * fromAsset.Asset.MarketData.CurrPrice;

        wallet.Transactions.Add(new()
        {
            Amount = amount,
            FromAsset = fromAsset.Asset,
            ToAsset = toAsset,
            Date = DateTime.UtcNow,
            TransactionType = TransactionType.Exchange,
        });

        if (fromAsset.Amount == 0 || fromAsset.Amount < 0.00001m)
        {
            wallet.Balances.Remove(fromAsset);
        }

        await _cryptoAssetRepository.SaveChangesAsync();

        List<CryptoBalanceDto> balances = _mapper.Map<List<CryptoBalanceDto>>(wallet.Balances);

        return new()
        {
            Status = ResponseStatus.Success,
            Data = balances,
        };
    }

    public async Task<ToClientDto> GetWalletBalancesAsync(Guid walletId) 
    {
        Wallet? wallet = (await _cryptoAssetRepository.GetWalletByIdAsync(walletId));
        if(wallet is null)
        {
            return new()
            {
                Message = "This wallet does not exist",
                Status = ResponseStatus.Success,
            };
        }

        List<CryptoBalance> balances = wallet.Balances;

        List<CryptoBalanceDto> balancesDto = _mapper.Map<List<CryptoBalanceDto>>(balances);

        ToClientDto output = new()
        {
            Data = balancesDto,
            Status = ResponseStatus.Success,
        };

        return output;
    }

    public async Task<ToClientDto> GetWalletStatisticAsync(Guid walletId)
    {
        Wallet? wallet = (await _cryptoAssetRepository.GetWalletByIdAsync(walletId));
        if (wallet is null)
        {
            return new()
            {
                Message = "This wallet does not exist",
                Status = ResponseStatus.Success,
            };
        }

        await _api.UpdateMarketDataInDbAsync(walletId);
        await UpdateWalletStatisticAsync(wallet);


        WalletStatistic statistic = wallet.Statistic;

        WalletStatisticDto staticticDto = _mapper.Map<WalletStatisticDto>(statistic);

        ToClientDto output = new()
        {
            Data = staticticDto,
            Status = ResponseStatus.Success,
        };

        return output;
    }

    public async Task<ToClientDto> GetWalletTransaction(Guid walletId)
    {
        Wallet? wallet = (await _cryptoAssetRepository.GetWalletByIdAsync(walletId));
        if (wallet is null)
        {
            return new()
            {
                Message = "This wallet does not exist",
                Status = ResponseStatus.Success,
            };
        }
        List<CryptoTransaction> history = wallet.Transactions.ToList();

        List<CryptoTransactionDto> historyDto = _mapper.Map<List<CryptoTransactionDto>>(history);

        ToClientDto output = new()
        {
            Data = historyDto,
            Status = ResponseStatus.Success,
        };
        return output;
    }
}
