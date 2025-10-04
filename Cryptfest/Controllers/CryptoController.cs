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

    [HttpGet("assets")]
    public async Task<IActionResult> GetAssets()
    {
        ToClientDto output = await _api.GetAssetsMarketDataAsync();
        return Ok(output);
    }

    [HttpGet("assets/{symbol}")]
    public async Task<IActionResult> GetAssetBySymbol(string symbol)
    {
        ToClientDto output = await _cryptoService.GetAssetBySymbolAsync(symbol);  
        return Ok(output);  
    }


    [HttpGet("wallets/{walletId}")]
    public async Task<IActionResult> GetWallet(int walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletAsync(walletId);
        return Ok(output);  
    }

    [HttpPut]
    public async Task<IActionResult> Deposit(int walletId, decimal amount)
    {
        ToClientDto output = await _cryptoService.EnsureDepositAsync(walletId, amount);
        return Ok(output);
    }

    //[HttpPost]
    //public async Task<IActionResult> ExecuteExchange(int walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount)
    //{
    //    return NoContent();
    //}
}

