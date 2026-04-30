using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class RemoveOrderItemCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
    }
}
