using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using AutoMapper;
using Cryptfest.Data.Entities.WalletEntities;
using Cryptfest.Model.Dtos;

namespace API.AutoMapperProfiles;

public class SharedAutoMapperProfiles : Profile
{
    public SharedAutoMapperProfiles()
    {
        CreateMap<Wallet,WalletDto>().ReverseMap();
        CreateMap<CryptoBalance, CryptoBalanceDto>().ReverseMap();
        CreateMap<WalletStatistic, WalletStatisticDto>().ReverseMap();
        CreateMap<CryptoAsset, CryptoAssetDto>().ReverseMap();
        CreateMap<CryptoAssetMarketData, CryptoAssetMarketDataDto>().ReverseMap();
    }
}
