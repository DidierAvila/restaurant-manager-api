using AutoMapper;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.Mappings.Restaurant;

public class OrderItemProfile : Profile
{
    public OrderItemProfile()
    {
        // Entity to DTO mappings
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.DishName, opt => opt.MapFrom(src => src.Dish != null ? src.Dish.Name : "Desconocido"))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        // DTO to Entity mappings
        CreateMap<AddOrderItemDto, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Dish, opt => opt.Ignore())
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()); // Se establece desde el precio del plato
            // AutoMapper mapea automáticamente: OrderId, DishId, Quantity, Notes
    }
}
