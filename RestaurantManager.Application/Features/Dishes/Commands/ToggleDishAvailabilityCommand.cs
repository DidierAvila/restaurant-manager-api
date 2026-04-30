using MediatR;
using RestaurantManager.Application.DTOs.Dishes;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class ToggleDishAvailabilityCommand : IRequest<DishDto>
    {
        public int Id { get; set; }
    }
}
