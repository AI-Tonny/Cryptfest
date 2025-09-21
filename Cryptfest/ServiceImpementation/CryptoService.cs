using API.Data;
using API.Data.Entities.Wallet;
using API.Interfaces.Services.Crypto;
using API.Model.Dtos;
using AutoMapper;
using Azure;
using Cryptfest.Enums;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Http;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cryptfest.ServiceImpementation;

public class CryptoService : ICryptoService
{
    private readonly ApplicationContext _context;
    private readonly IHttpClientFactory _httpClient;
    private readonly IMapper _mapper;

    public CryptoService(ApplicationContext context, IHttpClientFactory httpClient, IMapper mapper)
    {
        _context = context;
        _httpClient = httpClient;
        _mapper = mapper;
    }


    public async Task<ToClientDto> GetListOfAssetsAsync()
    {
        string url = "https://api.freecryptoapi.com/v1/getCryptoList?token=h5hof99iiun4za7lpfia";

        var client = _httpClient.CreateClient();
        string output = "";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();

            var resFromStream = JsonSerializer.Deserialize<GetListOfAssetsDto>(stream)!;

            if (output is null) 
            { 
                throw new InvalidOperationException ("External API returned invalid data"); 
            }

            var listOfAssets = _mapper.Map<List<CryptoAssetInfo>>(resFromStream.Result);

            var resultDto = new ToClientDto
            {
                Status = ResponseStatus.Success,
                Data = listOfAssets
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
}
