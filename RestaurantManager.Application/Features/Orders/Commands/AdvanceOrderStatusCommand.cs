using MediatR;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class AdvanceOrderStatusCommand : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
    }
}
