using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.DTOs.Dishes
{
    public class UpdateDishDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DishCategory Category { get; set; }
        public bool IsAvailable { get; set; }
    }
}
