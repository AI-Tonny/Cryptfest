using API.Data.Entities.Wallet;
using Cryptfest.Model.Dtos;

namespace Cryptfest.Interfaces.Services;

public interface IApiService
{
    Task<ToClientDto> GetAssetsWithPricesAsync();
}
