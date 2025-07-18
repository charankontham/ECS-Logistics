using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.Models;

namespace ECS_Logistics.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeliveryAgent, DeliveryAgentDto>();
        CreateMap<DeliveryAgentDto, DeliveryAgent>()
            .ForMember(dest => dest.DateAdded, opt => opt.Ignore())
            .ForMember(dest => dest.DateModified, opt => opt.Ignore());
    }
}