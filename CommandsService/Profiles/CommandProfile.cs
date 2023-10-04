using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDtos>();
            CreateMap<CommandCreateDtos, Command>();
            CreateMap<Command, CommandReadDtos>();
            CreateMap<PlatformPublishedDtos, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}   