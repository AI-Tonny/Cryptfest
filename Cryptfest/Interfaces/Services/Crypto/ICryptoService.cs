using Cryptfest.Model.Dtos;

namespace API.Interfaces.Services.Crypto;

public interface ICryptoService
{
    Task<ToClientDto> GetListOfAssetsAsync();
}
