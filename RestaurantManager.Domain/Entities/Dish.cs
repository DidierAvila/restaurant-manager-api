using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities;

[Table("menu_items", Schema = "Public")]
public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DishCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;
    public required DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation property
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
