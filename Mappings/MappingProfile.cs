using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Models;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeliveryAgent, DeliveryAgentDto>()
            .ForMember(dest => dest.Password, 
                opt => opt.Ignore());
        CreateMap<DeliveryAgentDto, DeliveryAgent>();
        CreateMap<DeliveryHub, DeliveryHubDto>();
        CreateMap<DeliveryHubDto, DeliveryHub>();
        CreateMap<DeliveryHub, DeliveryHubEnrichedDto>()
            .ForMember(dest => dest.DeliveryHubAddress, 
                opt => opt.MapFrom<AddressResolver>());
    }
}

public class AddressResolver(CustomerService customerService) : IValueResolver<DeliveryHub, DeliveryHubEnrichedDto, AddressDto>
{
    
    public AddressDto? Resolve(
        DeliveryHub source,
        DeliveryHubEnrichedDto destination, 
        AddressDto destMember,
        ResolutionContext context)
    {
        if (source.DeliveryHubAddressId == null)
            return null;
        return customerService.GetAddressById(source.DeliveryHubAddressId ?? 0).GetAwaiter().GetResult();
    }
}