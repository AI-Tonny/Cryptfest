using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using AutoMapper;
using Cryptfest.Data.Entities.WalletEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;

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

    public async Task<ToClientDto> GetWalletAsync(int walletId)
    {

        List<CryptoAssetDto> currentPrices = (List<CryptoAssetDto>)(await _api.GetAssetsMarketDataAsync()).Data!;
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


        decimal totalAssetsSum = 0;
        CryptoAssetDto? foundElement = null;

        // проходжуся по масиву активів, які є в гаманці
        foreach (var asset in wallet.Balances)
        {
            // треба знайти різницю між поточною ціною(assetListWithPrices) та ціною за яку був куплений актив(asset)
            foundElement = currentPrices.FirstOrDefault(x => x.Symbol == asset.Asset.Symbol);


            if (foundElement is null)
            {
                return new()
                {
                    Message = "this asset does not exist in db",
                    Status = ResponseStatus.Fail,
                };
            }
            totalAssetsSum += (foundElement.MarketData.CurrPrice * asset.Amount);

            asset.Asset.MarketData.CurrPrice = foundElement.MarketData.CurrPrice;  
            asset.Asset.MarketData.PercentChange1h = foundElement.MarketData.PercentChange1h;
            asset.Asset.MarketData.PercentChange24h = foundElement.MarketData.PercentChange24h;
            asset.Asset.MarketData.PercentChange7d = foundElement.MarketData.PercentChange7d;
            asset.Asset.MarketData.PercentChange30d = foundElement.MarketData.PercentChange30d;
            asset.Asset.MarketData.PercentChange60d = foundElement.MarketData.PercentChange60d;
        }


        wallet.Statistic.TotalDeposit = wallet.Statistic.TotalDeposit;
        wallet.Statistic.TotalAssets = wallet.Statistic.TotalAssets;
        wallet.Statistic.Apy = wallet.Statistic.TotalDeposit == 0 ?
            0
            :
            100 * (totalAssetsSum - wallet.Statistic.TotalDeposit) / (wallet.Statistic.TotalDeposit); // if(TotalDeposit == 0) { apy = 0; } else { apy =  100 * (totalAssetsSum ....)}

        WalletDto walletResult = _mapper.Map<Wallet, WalletDto>(wallet);  

        ToClientDto output;
        bool saveResult = await _cryptoAssetRepository.SaveChangesAsync();
        if (saveResult == true)
        {
            output = new()
            {
                Status = ResponseStatus.Success,
                Data = walletResult,
            };
        }
        else
        {
            output = new()
            {
                Status = ResponseStatus.Fail,
                Message = "Fail to save to db"
            };
        }
        return output;
    }

    public async Task<bool> CreateWallet(User user)
    {
        Wallet wallet = new()
        {
            User = user
        };
        
        await _cryptoAssetRepository.AddWalletAsync(wallet);
        bool isSaved = await _cryptoAssetRepository.SaveChangesAsync();

        return isSaved;
    }

    //public async Task<ToClientDto> Exchange(int walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount)
    //{
    //    Wallet? wallet = await _cryptoAssetRepository.GetWalletByIdAsync(walletId);
    //    if(wallet is null)
    //    {
    //        return new()
    //        {
    //            Message = "This wallet does not exist",
    //            Status = ResponseStatus.Fail,
    //        };
    //    }

    //    // does the fromAssetSymbol exist 
    //    CryptoBalance? fromAsset = wallet.Balances.FirstOrDefault(x => x.Asset.Symbol == fromAssetSymbol);

    //    // does the toAssetSymbol exist
    //    CryptoAsset? toAsset = await _cryptoAssetRepository.GetCryptoAssetBySymbolAsync(toAssetSymbol);
    //    if(fromAsset is null)
    //    {
    //        return new()
    //        {
    //            Message = "This asset does not exist in the user's wallet",
    //            Status = ResponseStatus.Fail,
    //        };
    //    }
    //    else if(fromAsset.Amount < amount)
    //    {
    //        return new()
    //        {
    //            Message = "The wallet does not have the amount of this asset",
    //            Status = ResponseStatus.Fail,
    //        };
    //    }
    //    else if(toAsset is null)
    //    {
    //        return new()
    //        {
    //            Message = "The asset does not exist in db",
    //            Status = ResponseStatus.Fail,
    //        };
    //    }


    //    decimal amountNewAssBal = (amount * fromAsset.Asset.MarketData.CurrPrice) / toAsset.MarketData.CurrPrice;
    //    decimal purchasePrice = toAsset.MarketData.CurrPrice;

    //    CryptoBalance newAssBal = new()
    //    {
    //        Amount = amountNewAssBal,
    //        Asset = toAsset,
    //        PurchasePrice = purchasePrice,
    //        Wallet = wallet,
    //    };

    //    wallet.Balances.Remove(fromAsset);
    //    wallet.Balances.Add(newAssBal);

    //    _cryptoAssetRepository.Update(wallet);

    //    // transfer already is done, only APY left to update; 
    //}
}
