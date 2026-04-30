using MediatR;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class CreateDishCommand : IRequest<Result<DishDto>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
