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
using Microsoft.AspNetCore.Authorization;

namespace Cryptfest.Controllers;

//[Authorize]
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

    [HttpGet("assets/{walletId}")]
    public async Task<IActionResult> GetAssets(Guid walletId)
    {
        ToClientDto output = await _cryptoService.GetAssetsAsync(walletId);
        return Ok(output);
    }

    [HttpGet("asset/{symbol}")]
    public async Task<IActionResult> GetAssetBySymbol(string symbol)
    {
        ToClientDto output = await _cryptoService.GetAssetBySymbolAsync(symbol);
        return Ok(output);
    }


    [HttpGet("wallets/{walletId}")]
    public async Task<IActionResult> GetWallet(Guid walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletAsync(walletId);
        return Ok(output);  
    }

    [HttpPut("deposit")]
    public async Task<IActionResult> Deposit( Guid walletId, decimal amount)
    {
        ToClientDto output = await _cryptoService.EnsureDepositAsync(walletId, amount);
        return Ok(output);
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> Exchange(Guid walletId, string fromAssetSymbol, string toAssetSymbol, decimal amount)
    {
        ToClientDto output = await _cryptoService.EnsureExchangeAsync(walletId, fromAssetSymbol, toAssetSymbol, amount);
        return Ok(output);
    }

    [HttpGet("wallet-balances")]
    public async Task<IActionResult> GetBalances(Guid walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletBalancesAsync(walletId);
        return Ok(output);
    }

    [HttpGet("wallet-statistic")]
    public async Task<IActionResult> GetStatistic(Guid walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletStatisticAsync(walletId);
        return Ok(output);
    }

    [HttpGet("wallet-transactions")]
    public async Task<IActionResult> GetTransactions(Guid walletId)
    {
        ToClientDto output = await _cryptoService.GetWalletTransaction(walletId);
        return Ok(output);
    }
}

