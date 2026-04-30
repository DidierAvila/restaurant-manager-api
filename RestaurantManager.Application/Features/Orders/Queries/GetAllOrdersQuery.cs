using MediatR;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Orders;

namespace RestaurantManager.Application.Features.Orders.Queries
{
    public class GetAllOrdersQuery : IRequest<PaginationResponseDto<OrderSummaryDto>>
    {
        public OrderFilterDto Filter { get; set; } = new();
    }
}
