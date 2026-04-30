using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Domain.Entities;

namespace RestaurantManager.Application.DTOs.Orders;

public class OrderFilterDto : PaginationRequestDto
{
    public int? TableNumber { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? OrderDateFrom { get; set; }
    public DateTime? OrderDateTo { get; set; }
    public string? Search { get; set; }
}
