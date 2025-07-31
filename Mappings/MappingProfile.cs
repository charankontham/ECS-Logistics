using AutoMapper;
using ECS_Logistics.DTOs;
using ECS_Logistics.FeignClients;
using ECS_Logistics.Models;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DeliveryAgentDto, DeliveryAgent>();
        CreateMap<DeliveryHub, DeliveryHubDto>();
        CreateMap<DeliveryHubDto, DeliveryHub>();
        CreateMap<OrderTrackingDto, OrderTracking>();
        CreateMap<DeliveryAgent, DeliveryAgentDto>()
            .ForMember(dest => dest.Password, 
                opt => opt.Ignore());
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
                opt => opt.Ignore())
            .ForMember(dest => dest.ProductId,
                opt => opt.MapFrom<ProductIdResolver>())
            .ForMember(dest => dest.BrandId,
                opt => opt.MapFrom<BrandIdResolver>())
            .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom<CategoryIdResolver>())
            .ForMember(dest => dest.SubCategoryId,
                opt => opt.MapFrom<SubCategoryIdResolver>());
        
        CreateMap<OrderReturn, OrderReturnDto>()
            .ForMember(dest => dest.PickupAddressId,
                opt => opt.Ignore());
        
        CreateMap<OrderReturn, OrderReturnEnrichedDto>()
            .ForMember(dest => dest.OrderTracking,
                opt => opt.MapFrom<OrderTrackingResolver>());
        
        CreateMap<OrderTracking, OrderTrackingEnrichedDto>()
            .ForMember(dest => dest.OrderTrackingId, 
                opt => 
                    opt.MapFrom(src => src.OrderTrackingId.ToString()))
            .ForMember(dest => dest.OrderItem, 
                opt => 
                    opt.MapFrom<OrderItemResolver>())
            .ForMember(dest => dest.Product, 
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

public class AddressResolver(CustomerService customerService) : 
    IValueResolver<DeliveryHub, DeliveryHubEnrichedDto, AddressDto?>
{
    
    public AddressDto? Resolve(
        DeliveryHub source,
        DeliveryHubEnrichedDto destination, 
        AddressDto? destMember,
        ResolutionContext context)
    {
        if (source.DeliveryHubAddressId == null)
            return null;
        return customerService.GetAddressById(source.DeliveryHubAddressId ?? 0).GetAwaiter().GetResult();
    }
}

public class OrderTrackingResolver(IOrderTrackingService orderTrackingService) : 
    IValueResolver<OrderReturn, OrderReturnEnrichedDto, OrderTrackingEnrichedDto?>
{
    public OrderTrackingEnrichedDto? Resolve(
        OrderReturn source, 
        OrderReturnEnrichedDto destination,
        OrderTrackingEnrichedDto? destMember, 
        ResolutionContext context)
    {
        var response = orderTrackingService.GetByIdAsync(source.OrderTrackingId ?? "").GetAwaiter().GetResult();
        if (response is OrderTrackingEnrichedDto dto)
        {
            return dto;
        }
        return null;
    }
}

public class OrderItemResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, OrderItemDto?>
{
    public OrderItemDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, OrderItemDto? destMember, ResolutionContext context)
    {
        return context.Items["OrderItem"] as OrderItemDto;
    }
}

public class ProductFinalResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, ProductFinalDto?>
{
    public ProductFinalDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, ProductFinalDto? destMember, ResolutionContext context)
    {
        return context.Items["Product"] as ProductFinalDto;
    }
}

public class DeliveryAgentResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, DeliveryAgentDto?>
{
    public DeliveryAgentDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, DeliveryAgentDto? destMember, ResolutionContext context)
    {
        return context.Items["DeliveryAgent"] as DeliveryAgentDto;
    }
}

public class DeliveryHubResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, DeliveryHubEnrichedDto?>
{
    public DeliveryHubEnrichedDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, DeliveryHubEnrichedDto? destMember, ResolutionContext context)
    {
        return context.Items["NearestHub"] as DeliveryHubEnrichedDto;
    }
}

public class CustomerAddressResolver : IValueResolver<OrderTracking, OrderTrackingEnrichedDto, AddressDto?>
{
    public AddressDto? Resolve(OrderTracking source, OrderTrackingEnrichedDto destination, AddressDto? destMember, ResolutionContext context)
    {
        return context.Items["CustomerAddress"] as AddressDto;
    }
}

public class ProductIdResolver : IValueResolver<OrderReturnDto, OrderReturn, int?>
{
    public int? Resolve(OrderReturnDto source, OrderReturn destination, int? destMember, ResolutionContext context)
    {
        return context.Items["ProductId"] as int?;
    }
}

public class CategoryIdResolver : IValueResolver<OrderReturnDto, OrderReturn, int?>
{
    public int? Resolve(OrderReturnDto source, OrderReturn destination, int? destMember, ResolutionContext context)
    {
        return context.Items["CategoryId"] as int?;
    }
}

public class SubCategoryIdResolver : IValueResolver<OrderReturnDto, OrderReturn, int?>
{
    public int? Resolve(OrderReturnDto source, OrderReturn destination, int? destMember, ResolutionContext context)
    {
        return context.Items["SubCategoryId"] as int?;
    }
}

public class BrandIdResolver : IValueResolver<OrderReturnDto, OrderReturn, int?>
{
    public int? Resolve(OrderReturnDto source, OrderReturn destination, int? destMember, ResolutionContext context)
    {
        return context.Items["BrandId"] as int?;
    }
}