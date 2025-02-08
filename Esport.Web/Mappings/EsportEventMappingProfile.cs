namespace Esport.Web.Mappings;

using AutoMapper;
using Domain.Models;
using Dtos;

public class EsportEventMappingProfile : Profile
{
    public EsportEventMappingProfile()
    {
        CreateMap<EsportEvent, EsportEventDto>()
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => src.Event ?? new Event()));

        CreateMap<Event, EventDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
            .ForMember(dest => dest.Market, opt => opt.MapFrom(src => src.Market))
            .ReverseMap();

        CreateMap<Participant, ParticipantDto>().ReverseMap();
        CreateMap<Market, MarketDto>()
            .ForMember(dest => dest.Selections, opt => opt.MapFrom(src => src.Selections))
            .ReverseMap();

        CreateMap<Selection, SelectionDto>().ReverseMap();
    }
}

