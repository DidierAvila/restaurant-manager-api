namespace RestaurantManager.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int TableNumber { get; set; }
        public string Waiter { get; set; } = string.Empty;
    }
}
