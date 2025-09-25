using API.Data.Entities.Wallet;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using Cryptfest.Enums;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Cryptfest.Data.Entities.WalletEntities;
using API.Data.Entities.WalletEntities;
using API.Data;
using Cryptfest.Interfaces.Services;

namespace Cryptfest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CryptoController : ControllerBase
{
    private readonly ICryptoService _cryptoService;
    private readonly IApiService _api;

    public CryptoController(ICryptoService cryptoService, IApiService api)
    {
        _cryptoService = cryptoService;
        _api = api;
    }

    [HttpGet("GetListOfAssets")]
    public async Task<IActionResult> GetListOfAssets()
    {
        ToClientDto output = await _api.GetAssetsWithPricesAsync();
        return Ok(output);
    }



    [HttpGet("GetAssetBySymbol/{symbol}")]
    public async Task<IActionResult> GetAssetBySymbol(string symbol)
    {
        ToClientDto output = await _cryptoService.GetAssetBySymbolAsync(symbol);  
        return Ok(output);  
    }



    [HttpGet("GetWallet/{walletId}")]
    public async Task<IActionResult> GetWallet(int walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletAsync(walletId);
        Console.WriteLine(output.Data);
        return Ok(output);  
    }
}

