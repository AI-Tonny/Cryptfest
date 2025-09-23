using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using API.Interfaces.Services.Crypto;
using AutoMapper;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Model.Dtos;
using System.Text.Json;

namespace Cryptfest.ServiceImpementation;

public class CryptoService : ICryptoService
{
    private readonly ICryptoAssetRopository _cryptoAssetRopository;
    private readonly IHttpClientFactory _httpClient;
    private readonly IMapper _mapper;

    public CryptoService(ICryptoAssetRopository cryptoAssetRopository, IHttpClientFactory httpClient, IMapper mapper)
    {
        _cryptoAssetRopository = cryptoAssetRopository;
        _httpClient = httpClient;
        _mapper = mapper;
    }

    public async Task<ToClientDto> GetListOfAssetsWithPricesAsync()
    {

        List<CryptoAssetInfo> cryptoAssets = await _cryptoAssetRopository.GetCryptoAssetsAsync();

        HttpClient client = _httpClient.CreateClient();

        var keyAndToken = _cryptoAssetRopository.GetApiAccess();  
        client.DefaultRequestHeaders.Add($"{keyAndToken.Key}",$"{keyAndToken.Token}");
        string url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var receivedJson = await response.Content.ReadAsStringAsync();

            JsonDocument doc = JsonDocument.Parse(receivedJson);
            JsonElement json = doc.RootElement;

            var data = json.GetProperty("data");

            // variables for loop 
            CryptoAssetInfo? asset = new();
            decimal price, PercentChange1h, PercentChange24h, PercentChange7d, PercentChange30d, PercentChange60d;
            JsonElement forPrice;

            foreach (var item in data.EnumerateArray())
            {
                string symbol = item.GetProperty("symbol").GetString()!;
                asset = await _cryptoAssetRopository.GetCryptoAssetBySymbolAsync(symbol);
                if (asset is not null)
                {
                    forPrice = item.GetProperty("quote").GetProperty("USD");
                    forPrice.GetProperty("price").TryGetDecimal(out price);
                    forPrice.GetProperty("percent_change_1h").TryGetDecimal(out PercentChange1h);
                    forPrice.GetProperty("percent_change_24h").TryGetDecimal(out PercentChange24h);
                    forPrice.GetProperty("percent_change_7d").TryGetDecimal(out PercentChange7d);
                    forPrice.GetProperty("percent_change_30d").TryGetDecimal(out PercentChange30d);
                    forPrice.GetProperty("percent_change_60d").TryGetDecimal(out PercentChange60d);


                    asset.Price = new CryptoAssetPrice()
                    {
                        Price = price,
                        PercentChange1h = PercentChange1h,
                        PercentChange24h = PercentChange24h,
                        PercentChange7d = PercentChange7d,
                        PercentChange30d = PercentChange30d,
                        PercentChange60d = PercentChange60d
                    };
                    
                }
            }

            ToClientDto resultDto = new ToClientDto()
            {
                Status = ResponseStatus.Success,
                Data = cryptoAssets
            };
            return resultDto;
        }
        catch (HttpRequestException ex)
        {

            ToClientDto errorDto = new ToClientDto
            {
                Message = "Failed to get data from external API",
                Status = ResponseStatus.Fail,
            };

            return errorDto;
        }

        catch (Exception ex)
        {
            var errorDto = new ToClientDto
            {
                Message = $"Unexpected error: {ex.Message}",
                Status = ResponseStatus.Fail,
            };
            return errorDto;
        }
    }


    public async Task<ToClientDto> GetAssetBySymbolAsync(string symbol)
    {
        CryptoAssetInfo? output = await _cryptoAssetRopository.GetCryptoAssetBySymbolAsync(symbol);
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
                Message = "this asset does not exists"
            };
        }
    }

}
