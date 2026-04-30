namespace RestaurantManager.Application.DTOs.Orders
{
    public class AddOrderItemDto
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
