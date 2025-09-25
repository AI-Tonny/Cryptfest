using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using AutoMapper;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model.Dtos;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Cryptfest.ServiceImpementation;

public class CryptoService : ICryptoService
{
    private readonly ICryptoAssetRepository _cryptoAssetRepository;
    private readonly IApiService _api;

    public CryptoService(ICryptoAssetRepository cryptoAssetRepository, IHttpClientFactory httpClient, IMapper mapper, IApiService api)
    {
        _cryptoAssetRepository = cryptoAssetRepository;
        _api = api;
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

        List<CryptoAsset> currentPrices = (List<CryptoAsset>)(await _api.GetAssetsWithPricesAsync()).Data!;
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
        CryptoAsset? foundElement = null;

        // проходжусь по масиву активів, які є в гаманці
        foreach (var asset in wallet.Balances!)
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

        wallet.Statistic = new()
        {
            TotalDeposit = wallet.Statistic.TotalDeposit,
            TotalAssets = totalAssetsSum,
            Apy = 100 * (totalAssetsSum - wallet.Statistic.TotalDeposit) / (wallet.Statistic.TotalDeposit)
        };


        ToClientDto output;
        bool saveResult = await _cryptoAssetRepository.SaveChangesAsync();
        if (saveResult == true)
        {
            output = new()
            {
                Status = ResponseStatus.Success,
                Data = wallet,
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


}
