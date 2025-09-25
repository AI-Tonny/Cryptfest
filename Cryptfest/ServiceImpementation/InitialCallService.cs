using API.Data;
using API.Data.Entities.Wallet;
using Cryptfest.Data.Entities.Api;
using Cryptfest.Interfaces.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text.Json;

namespace Cryptfest.ServiceImpementation;

public class InitialCallService : IInitialCallService
{

    private readonly ApplicationContext _context;
    private readonly IHttpClientFactory _httpClient;
    //private readonly ApiAccess _api;
    public InitialCallService(ApplicationContext context, IHttpClientFactory httpClient)
    {
        _context = context;
        _httpClient = httpClient;
        //_api = _context.ApiAccess.ToList().First(); 
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
        string url = "https://pro-api.coinmarketcap.com/v1/cryptocurrency/map?sort=cmc_rank&limit=30";

        var client = _httpClient.CreateClient();
        //client.DefaultRequestHeaders.Add($"{_api.Key}", $"{_api.Token}");
        client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", "fb2a0246-a49a-4299-929a-75de33fb37ec");

        try
        {

            string json = await client.GetStringAsync(url);

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            JsonElement items = root.GetProperty("data");
            string symbol = "";
            string name = "";
            string logo = "";

            List<CryptoAsset> assetsInfo = new List<CryptoAsset>();

            foreach (var item in items.EnumerateArray())
            {
                symbol = item.GetProperty("symbol").GetString() ?? "";
                name = item.GetProperty("name").GetString() ?? "";


                assetsInfo.Add(new CryptoAsset
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
                MaxDegreeOfParallelism = 5,
            };

            await Parallel.ForEachAsync(assetsInfo, options, async (symbolName, token) =>
            {
                logo = await GetAssetLogo(symbolName.Symbol);
                if (!string.IsNullOrEmpty(logo))
                {
                    logos[symbolName.Symbol] = logo;
                }
                
            });

            foreach (var item in assetsInfo)
            {
                if (logos.TryGetValue(item.Symbol, out logo))
                {
                    item.Logo = logo;
                }
            }
            await _context.CryptoAssetInfo.AddRangeAsync(assetsInfo);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex) { throw; }

    }

    public async Task InitialApiAccess()
    {
        ApiAccess apiAccess = new ApiAccess()
        {
            Key = "X-CMC_PRO_API_KEY",
            Token = "fb2a0246-a49a-4299-929a-75de33fb37ec"
        };
        await _context.AddAsync(apiAccess);  
        await _context.SaveChangesAsync();  
    }
}
