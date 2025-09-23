using API.Data;
using API.Data.Entities.Wallet;
using API.Interfaces.Services.Crypto;
using AutoMapper;
using Cryptfest.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;

namespace Cryptfest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CryptoController : ControllerBase
{
    private readonly ICryptoService _cryptoService;

    public CryptoController(ICryptoService cryptoService)
    {
        _cryptoService = cryptoService;
    }

    // if u want use it, uncomment SaveAssetsInDbFromApi() and InitialApiAccess() in Program.cs
    [HttpGet("GetListOfAssets")]
    public async Task<IActionResult> GetListOfAssets()
    {
        ToClientDto output = await _cryptoService.GetListOfAssetsWithPricesAsync();
        return Ok(output);
    }

    [HttpGet("GetAssetBySymbol/{symbol}")]
    public async Task<IActionResult> GetAssetBySymbol(string symbol)
    {
        
    }
}
