using AutoMapper;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.Mappings.Restaurant;

public class DishProfile : Profile
{
    public DishProfile()
    {
        // Entity to DTO mappings
        CreateMap<Dish, DishDto>()
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
            .ForMember(dest => dest.CategoryEnum, opt => opt.MapFrom(src => src.Category));

        // DTO to Entity mappings
        CreateMap<CreateDishDto, Dish>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
            // AutoMapper mapea automáticamente: Name, Description, Price, Category, IsAvailable

        CreateMap<UpdateDishDto, Dish>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItems, opt => opt.Ignore());
            // AutoMapper mapea automáticamente: Id, Name, Description, Price, Category, IsAvailable
    }
}
