
using PlatformService.Dtos;
using PlatformService.Modles;
using AutoMapper;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            CreateMap<Platform, PlatformReadDtos>();
            CreateMap<PlatformCreateDtos, Platform>();
            CreateMap<PlatformReadDtos, PlatformPublishDto>();
        }
    }
}