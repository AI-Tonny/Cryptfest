using API.Data.Entities.Wallet;
using Cryptfest.Model.Dtos;

namespace API.Interfaces.Services.Crypto;

public interface ICryptoService
{
    Task<ToClientDto> GetListOfAssetsWithPricesAsync();
    //Task<ToClientDto> GetAssetBySymbolAsync();
}
