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
        CreateMap<CryptoTransaction, CryptoTransactionDto>()
            .ForMember(dest => dest.FromAsset,
                opt => opt.MapFrom(src => src.FromAsset!.Symbol))
            .ForMember(dest => dest.ToAsset,
                opt => opt.MapFrom(src => src.ToAsset!.Symbol));
    }
}
