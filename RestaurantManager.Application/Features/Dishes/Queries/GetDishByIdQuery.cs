using MediatR;
using RestaurantManager.Application.DTOs.Dishes;

namespace RestaurantManager.Application.Features.Dishes.Queries
{
    public class GetDishByIdQuery : IRequest<DishDto?>
    {
        public int Id { get; set; }
    }
}
