using MediatR;
using RestaurantManager.Application.DTOs.Dishes;

namespace RestaurantManager.Application.Features.Dishes.Commands
{
    public class UpdateDishCommand : IRequest<DishDto>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public bool IsAvailable { get; set; }
    }
}
