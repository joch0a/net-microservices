using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService.Protos;

namespace CommandsService.Profiles
{
    public class CommandsProfiles : Profile
    {
        public CommandsProfiles()
        {
            // Source -> Target

            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();

            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalId, opts => opts.MapFrom(src => src.PlatformId))
                // this is not needed... but I want to be consistent with the video
                .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name)); 
        }
    }
}
