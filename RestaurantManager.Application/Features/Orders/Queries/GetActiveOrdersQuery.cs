using MediatR;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Queries
{
    public class GetActiveOrdersQuery : IRequest<List<OrderSummaryDto>>
    {
    }
}
