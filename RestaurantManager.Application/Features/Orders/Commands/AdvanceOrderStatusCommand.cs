using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class AdvanceOrderStatusCommand : IRequest<OrderDto>
    {
        public int OrderId { get; set; }
    }
}
