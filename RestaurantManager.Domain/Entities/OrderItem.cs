using System;

namespace RestaurantManager.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Notes { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public Dish Dish { get; set; } = null!;

        // Calculated property
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
