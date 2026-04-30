using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class AddOrderItemCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
