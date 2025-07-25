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
        CreateMap<OrderReturnDto, OrderReturn>()
            .ForMember(dest => dest.BrandId,
                opt => opt.Ignore())
            .ForMember(dest => dest.ProductId,
                opt => opt.Ignore())
            .ForMember(dest => dest.CategoryId,
                opt => opt.Ignore())
            .ForMember(dest => dest.SubCategoryId,
                opt => opt.Ignore());
        CreateMap<OrderReturn, OrderReturnDto>()
            .ForMember(dest => dest.OrderItemId,
                opt => opt.Ignore())
            .ForMember(dest => dest.PickupAddressId,
                opt => opt.Ignore());
        CreateMap<OrderReturn, OrderReturnEnrichedDto>()
            .ForMember(dest => dest.OrderTracking,
                opt => opt.MapFrom<OrderTrackingResolver>());
        CreateMap<OrderTrackingDto, OrderTracking>();
        CreateMap<OrderTracking, OrderTrackingEnrichedDto>()
            .ForMember(dest => dest.OrderTrackingId, 
                opt => 
                    opt.MapFrom(src => src.OrderTrackingId.ToString()))
            .ForMember(dest => dest.OrderItem, 
                opt => 
                    opt.MapFrom<ProductFinalResolver>())
            .ForMember(dest => dest.DeliveryAgent, 
                opt => 
                    opt.MapFrom<DeliveryAgentResolver>())
            .ForMember(dest => dest.NearestHub, 
                opt => 
                    opt.MapFrom<DeliveryHubResolver>())
            .ForMember(dest => dest.CustomerAddress, 
                opt => 
                    opt.MapFrom<CustomerAddressResolver>());
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

public class OrderTrackingResolver : IValueResolver<OrderReturn, OrderReturnEnrichedDto, OrderTrackingEnrichedDto>
{
    public OrderTrackingEnrichedDto Resolve(
        OrderReturn source, 
        OrderReturnEnrichedDto destination,
        OrderTrackingEnrichedDto destMember, 
        ResolutionContext context)
    {
        throw new NotImplementedException();
    }
}

public class ProductFinalResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, ProductFinalDto>
{
    public ProductFinalDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, ProductFinalDto destMember, ResolutionContext context)
    {
        return context.Items["OrderItem"] as ProductFinalDto;
    }
}

public class DeliveryAgentResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, DeliveryAgentDto>
{
    public DeliveryAgentDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, DeliveryAgentDto destMember, ResolutionContext context)
    {
        return context.Items["DeliveryAgent"] as DeliveryAgentDto;
    }
}

public class DeliveryHubResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, DeliveryHubDto>
{
    public DeliveryHubDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, DeliveryHubDto destMember, ResolutionContext context)
    {
        return context.Items["NearestHub"] as DeliveryHubDto;
    }
}

public class CustomerAddressResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, AddressDto>
{
    public AddressDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, AddressDto destMember, ResolutionContext context)
    {
        return context.Items["CustomerAddress"] as AddressDto;
    }
}