using API.Data;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Cryptfest.ServiceImplementation;

public class InitialCallService : IInitialCallService
{

    private readonly ApplicationContext _context;
    private readonly IHttpClientFactory _httpClient;
    private readonly ICryptoAssetRepository _cryptoAssetRepository;
    private readonly IApiService _api;

    public InitialCallService(ApplicationContext context, IHttpClientFactory httpClient, ICryptoAssetRepository cryptoAssetRepository, IApiService api)
    {
        _context = context;
        _httpClient = httpClient;
        _cryptoAssetRepository = cryptoAssetRepository;
        _api = api;
    }

    private async Task<string> GetAssetLogo(string symbol)
    {
        //var keyAndToken = _context.ApiAccess.ToList().First();

        string url = $"https://pro-api.coinmarketcap.com/v2/cryptocurrency/info?symbol={symbol}";
        HttpClient client = _httpClient.CreateClient(url);
        //client.DefaultRequestHeaders.Add($"{_api.Key}", $"{_api.Token}");
        client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", "fb2a0246-a49a-4299-929a-75de33fb37ec");

        try
        {
            string json = await client.GetStringAsync(url);

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement jsonElement = doc.RootElement;
            string output = jsonElement.GetProperty($"data").GetProperty($"{symbol.ToUpper()}")[0].GetProperty("logo").GetString()!;
            return output;
        }
        catch (HttpRequestException ex)
        {
            return null;
        }
    }

    public async Task<bool> SaveAssetsInDbFromApi()
    {
        var client = _httpClient.CreateClient();

        var keyAndToken = _api.GetApiKeyToken();

        string top30 = _api.GetTop30AssetUrl();
        string latestDataUrl = _api.GetLatestDataUrl();

        client.DefaultRequestHeaders.Add($"{keyAndToken.Key}", $"{keyAndToken.Token}");

        try
        {

            string json = await client.GetStringAsync(top30);

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // json where all info about assets
            JsonElement items = root.GetProperty("data");
            
            string symbol = "";
            string name = "";
            string logo = "";

            List<CryptoAsset> assets = new List<CryptoAsset>();

            foreach (var item in items.EnumerateArray())
            {
                symbol = item.GetProperty("symbol").GetString() ?? "";
                name = item.GetProperty("name").GetString() ?? "";


                assets.Add(new CryptoAsset
                {
                    Name = name,
                    Symbol = symbol,
                    MarketData = new()
                    {
                        CurrPrice = 0
                    }
                });
            }

            ConcurrentDictionary<string, string> logos = new ConcurrentDictionary<string, string>();

            ParallelOptions options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 2,
            };

            await Parallel.ForEachAsync(assets, options, async (symbolName, token) =>
            {
                logo = await GetAssetLogo(symbolName.Symbol);
                if (!string.IsNullOrEmpty(logo))
                {
                    logos[symbolName.Symbol] = logo;
                }
                
            });

            foreach (var item in assets)
            {
                if (logos.TryGetValue(item.Symbol, out logo))
                {
                    item.Logo = logo;
                }
            }

            // Because will be error 429(too many requests)
            await Task.Delay(60_000);


            // here taking info about price

            var listingsJson = await client.GetStringAsync(latestDataUrl);
            var listingsDoc = JsonDocument.Parse(listingsJson);
            var listingsData = listingsDoc.RootElement.GetProperty("data");

            foreach (var item in listingsData.EnumerateArray())
            {
                symbol = item.GetProperty("symbol").GetString()!;
                var asset = assets.FirstOrDefault(x => x.Symbol == symbol);
                if (asset is not null)
                {
                    var usd = item.GetProperty("quote").GetProperty("USD");
                    usd.GetProperty("price").TryGetDecimal(out var price);
                    usd.GetProperty("percent_change_1h").TryGetDecimal(out var percent1h);
                    usd.GetProperty("percent_change_24h").TryGetDecimal(out var percent24h);
                    usd.GetProperty("percent_change_7d").TryGetDecimal(out var percent7d);
                    usd.GetProperty("percent_change_30d").TryGetDecimal(out var percent30d);
                    usd.GetProperty("percent_change_60d").TryGetDecimal(out var percent60d);


                    asset.MarketData = new CryptoAssetMarketData
                    {
                        CurrPrice = price,
                        PercentChange1h = percent1h,
                        PercentChange24h = percent24h,
                        PercentChange7d = percent7d,
                        PercentChange30d = percent7d,
                        PercentChange60d = percent7d
                    };
                }
            }

            foreach (var asset in assets)
            {
                var existing = await _context.CryptoAsset.FirstOrDefaultAsync(x => x.Symbol == asset.Symbol);
                if (existing == null)
                {
                    await _context.CryptoAsset.AddAsync(asset);
                }
                else
                {
                    existing.Name = asset.Name;
                    existing.Logo = asset.Logo;
                    existing.MarketData = asset.MarketData;
                    _context.CryptoAsset.Update(existing);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        catch (Exception ex) { throw; }

    }
}
