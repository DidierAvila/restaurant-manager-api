using MediatR;
using RestaurantManager.Application.DTOs.Dishes;

namespace RestaurantManager.Application.Features.Dishes.Queries
{
    public class GetAvailableDishesQuery : IRequest<List<DishDto>>
    {
    }
}
