using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Queries
{
    public class GetOrderByIdQuery : IRequest<OrderDto?>
    {
        public int Id { get; set; }
    }
}
