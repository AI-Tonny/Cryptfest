using API.Data;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using API.Interfaces.Services.Crypto;
using AutoMapper;
using Cryptfest.Enums;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Cryptfest.ServiceImpementation;

public class CryptoService : ICryptoService
{
    private readonly ApplicationContext _context;
    private readonly IHttpClientFactory _httpClient;
    private readonly IMapper _mapper;

    public CryptoService(ApplicationContext context, IHttpClientFactory httpClient, IMapper mapper )
    {
        _context = context;
        _httpClient = httpClient;
        _mapper = mapper;
    }


    public async Task<ToClientDto> GetListOfAssetsWithPricesAsync()
    {

        List<CryptoAssetInfo> cryptoAssets = await _context.CryptoAssetInfo.ToListAsync();

        HttpClient client = _httpClient.CreateClient();
        
        var keyAndToken = _context.ApiAccess.ToList().First();  
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
                asset = await _context.CryptoAssetInfo.FirstOrDefaultAsync(x => x.Symbol == symbol);
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


    //public async Task<ToClientDto> GetAssetBySymbolAsync(string symbol)
    //{
    //    string token = "h5hof99iiun4za7lpfia";
    //    string url = "https://api.freecryptoapi.com/v1/getData?symbol={symbol}&token={token}";

    //    HttpClient client = _httpClient.CreateClient();

    //    try
    //    {
    //        HttpResponseMessage response = await client.GetAsync(url);
    //        response.EnsureSuccessStatusCode();

    //        using Stream stream = response.Content.ReadAsStream();

    //        string output = JsonSerializer.Serialize(stream);
    //    }
    //    catch
    //    {

    //    }
    //}
}
