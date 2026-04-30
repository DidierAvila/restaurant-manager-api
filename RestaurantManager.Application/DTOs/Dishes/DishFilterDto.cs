using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.DTOs.Dishes
{
    public class DishFilterDto : PaginationRequestDto
    {
        public DishCategory? Category { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Search { get; set; }
    }
}
