using MediatR;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Result<OrderDto>>
    {
        public int TableNumber { get; set; }
        public string Waiter { get; set; } = string.Empty;
    }
}
