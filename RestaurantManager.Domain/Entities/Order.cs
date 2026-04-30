using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities;

[Table("Order", Schema = "Public")]
public class Order
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public string Waiter { get; set; } = string.Empty;
    public required DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Abierto;

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    // Calculated property
    public decimal Total => OrderItems?.Sum(item => item.Subtotal) ?? 0;
}
