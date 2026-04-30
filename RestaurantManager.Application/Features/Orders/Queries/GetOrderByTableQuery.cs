using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Queries
{
    public class GetOrderByTableQuery : IRequest<OrderDto?>
    {
        public int TableNumber { get; set; }
    }
}
