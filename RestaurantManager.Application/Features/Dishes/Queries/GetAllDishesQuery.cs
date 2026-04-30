using MediatR;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.Features.Dishes.Queries
{
    public class GetAllDishesQuery : IRequest<PaginationResponseDto<DishDto>>
    {
        public DishFilterDto Filter { get; set; } = new();
    }
}
