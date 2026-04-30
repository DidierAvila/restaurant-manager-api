using MediatR;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.Features.Dishes.Queries
{
    public class GetDishesByCategoryQuery : IRequest<List<DishDto>>
    {
        public DishCategory Category { get; set; }
    }
}
