using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.Models;

namespace ECS_Logistics.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeliveryAgent, DeliveryAgentDto>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());
        CreateMap<DeliveryAgentDto, DeliveryAgent>();
    }
}