using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string Waiter { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public OrderStatus StatusEnum { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
