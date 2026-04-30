using AutoMapper;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.Mappings.Restaurant;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        // Entity to DTO mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StatusEnum, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Subtotal)))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StatusEnum, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.OrderItems.Sum(oi => oi.Subtotal)))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));

        // DTO to Entity mappings
        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Abierto))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
            // AutoMapper mapea automáticamente: TableNumber, Waiter
    }
}
