using MediatR;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.Orders.Commands
{
    public class AddOrderItemCommand : IRequest<Result<OrderDto>>
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
    }
}
