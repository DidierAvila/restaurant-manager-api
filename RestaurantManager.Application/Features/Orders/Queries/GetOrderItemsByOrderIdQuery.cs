using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Queries
{
    public class GetOrderItemsByOrderIdQuery : IRequest<List<OrderItemDto>?>
    {
        public int OrderId { get; set; }
    }
}
