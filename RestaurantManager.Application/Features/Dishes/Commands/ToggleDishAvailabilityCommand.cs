using MediatR;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class ToggleDishAvailabilityCommand : IRequest<Result<DishDto>>
    {
        public int Id { get; set; }
    }
}
