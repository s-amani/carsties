using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDTO>()
            .IncludeMembers(x => x.Item);

        CreateMap<Item, AuctionDTO>();

        CreateMap<CreateAuctionDTO, Auction>()
            .ForMember(x => x.Item, o => o.MapFrom(c => c));
            
        CreateMap<CreateAuctionDTO, Item>();
        
        CreateMap<AuctionDTO, AuctionCreated>();
    }
}
