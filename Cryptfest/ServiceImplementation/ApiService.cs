using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Enums;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Model.Dtos;
using System.Net.Http;
using System.Text.Json;

namespace Cryptfest.ServiceImpementation;

public class ApiService : IApiService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ICryptoAssetRepository _cryptoAssetRepository;

    public ApiService(IHttpClientFactory httpClient, ICryptoAssetRepository cryptoAssetRepository)
    {
        _httpClient = httpClient;
        _cryptoAssetRepository = cryptoAssetRepository;
    }

    public async Task<ToClientDto> GetAssetsWithPricesAsync()
    {
        List<CryptoAsset> cryptoAssets = await _cryptoAssetRepository.GetCryptoAssetsAsync();

        HttpClient client = _httpClient.CreateClient();

        var keyAndToken = _cryptoAssetRepository.GetApiAccess();
        client.DefaultRequestHeaders.Add($"{keyAndToken.Key}", $"{keyAndToken.Token}");
        string url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var receivedJson = await response.Content.ReadAsStringAsync();

            JsonElement doc = JsonDocument.Parse(receivedJson).RootElement;

            var jsonData = doc.GetProperty("data");

            // variables for loop 
            CryptoAsset? asset = null;
            decimal price, PercentChange1h, PercentChange24h, PercentChange7d, PercentChange30d, PercentChange60d;
            JsonElement forPrice;


            foreach (var item in jsonData.EnumerateArray())
            {
                string symbol = item.GetProperty("symbol").GetString()!;
                asset = cryptoAssets.FirstOrDefault(x => x.Symbol == symbol);

                if (asset is not null)
                {
                    forPrice = item.GetProperty("quote").GetProperty("USD");
                    forPrice.GetProperty("price").TryGetDecimal(out price);
                    forPrice.GetProperty("percent_change_1h").TryGetDecimal(out PercentChange1h);
                    forPrice.GetProperty("percent_change_24h").TryGetDecimal(out PercentChange24h);
                    forPrice.GetProperty("percent_change_7d").TryGetDecimal(out PercentChange7d);
                    forPrice.GetProperty("percent_change_30d").TryGetDecimal(out PercentChange30d);
                    forPrice.GetProperty("percent_change_60d").TryGetDecimal(out PercentChange60d);


                    asset.MarketData = new CryptoAssetMarketData()
                    {
                        CurrPrice = price,
                        PercentChange1h = PercentChange1h,
                        PercentChange24h = PercentChange24h,
                        PercentChange7d = PercentChange7d,
                        PercentChange30d = PercentChange30d,
                        PercentChange60d = PercentChange60d,
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
            throw;
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
}
