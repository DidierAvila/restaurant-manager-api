using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public int TableNumber { get; set; }
        public string Waiter { get; set; } = string.Empty;
    }
}
