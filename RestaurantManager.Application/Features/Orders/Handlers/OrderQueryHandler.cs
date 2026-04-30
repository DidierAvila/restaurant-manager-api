using MediatR;
using Microsoft.EntityFrameworkCore;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Application.Features.Common.Pagination;
using RestaurantManager.Application.Features.Orders.Queries;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Application.Features.Orders.Handlers
{
    public class OrderQueryHandler :
        IRequestHandler<GetAllOrdersQuery, PaginationResponseDto<OrderSummaryDto>>,
        IRequestHandler<GetOrderByIdQuery, OrderDto?>,
        IRequestHandler<GetActiveOrdersQuery, List<OrderSummaryDto>>,
        IRequestHandler<GetOrderByTableQuery, OrderDto?>
    {
        private readonly IRepositoryBase<Order> _orderRepository;
        private readonly IRepositoryBase<OrderItem> _orderItemRepository;
        private readonly IRepositoryBase<Dish> _dishRepository;
        private readonly RestaurantManagerDbContext _context;

        public OrderQueryHandler(
            IRepositoryBase<Order> orderRepository,
            IRepositoryBase<OrderItem> orderItemRepository,
            IRepositoryBase<Dish> dishRepository,
            RestaurantManagerDbContext context)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
            _context = context;
        }

        public async Task<PaginationResponseDto<OrderSummaryDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var filter = request.Filter ?? new OrderFilterDto();

            var page = filter.Page <= 0 ? 1 : filter.Page;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Orders.AsNoTracking().AsQueryable();

            if (filter.TableNumber.HasValue)
            {
                query = query.Where(o => o.TableNumber == filter.TableNumber.Value);
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(o => o.Status == filter.Status.Value);
            }

            if (filter.OrderDateFrom.HasValue)
            {
                query = query.Where(o => o.OrderDate >= filter.OrderDateFrom.Value);
            }

            if (filter.OrderDateTo.HasValue)
            {
                query = query.Where(o => o.OrderDate <= filter.OrderDateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(o => o.Waiter.Contains(search));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            var isDefaultSort = string.IsNullOrWhiteSpace(filter.SortBy);
            var sortBy = isDefaultSort ? "orderdate" : filter.SortBy;
            var sortDescending = isDefaultSort ? true : filter.SortDescending;

            var sortedQuery = SortingHelper.CreateSortingBuilder(query)
                .AddSortMapping("id", o => o.Id)
                .AddSortMapping("tablenumber", o => o.TableNumber)
                .AddSortMapping("waiter", o => o.Waiter)
                .AddSortMapping("orderdate", o => o.OrderDate)
                .AddSortMapping("status", o => o.Status)
                .SetDefaultSort(o => o.OrderDate)
                .ApplySorting(sortBy, sortDescending);

            var pageData = await sortedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    TableNumber = o.TableNumber,
                    Waiter = o.Waiter,
                    OrderDate = o.OrderDate,
                    StatusEnum = o.Status,
                    Total = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    ItemCount = o.OrderItems.Count()
                })
                .ToListAsync(cancellationToken);

            foreach (var dto in pageData)
            {
                dto.Status = dto.StatusEnum.ToString();
            }

            return pageData.ToPaginatedResult(page, pageSize, totalRecords, sortBy);
        }

        public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByID(request.Id, cancellationToken);
            if (order == null)
                return null;

            return await MapToDtoWithItems(order, cancellationToken);
        }

        public async Task<List<OrderSummaryDto>> Handle(GetActiveOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.Finds(
                o => o.Status != OrderStatus.Entregado && o.Status != OrderStatus.Cerrado,
                cancellationToken);

            if (orders == null)
                return new List<OrderSummaryDto>();

            var summaries = new List<OrderSummaryDto>();

            foreach (var order in orders.OrderByDescending(o => o.OrderDate))
            {
                var items = await _orderItemRepository.Finds(
                    oi => oi.OrderId == order.Id,
                    cancellationToken);

                var total = items?.Sum(i => i.Subtotal) ?? 0;
                var itemCount = items?.Count() ?? 0;

                summaries.Add(new OrderSummaryDto
                {
                    Id = order.Id,
                    TableNumber = order.TableNumber,
                    Waiter = order.Waiter,
                    OrderDate = order.OrderDate,
                    Status = order.Status.ToString(),
                    StatusEnum = order.Status,
                    Total = total,
                    ItemCount = itemCount
                });
            }

            return summaries;
        }

        public async Task<OrderDto?> Handle(GetOrderByTableQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.Find(
                o => o.TableNumber == request.TableNumber &&
                     o.Status != OrderStatus.Entregado &&
                     o.Status != OrderStatus.Cerrado,
                cancellationToken);

            if (order == null)
                return null;

            return await MapToDtoWithItems(order, cancellationToken);
        }

        private async Task<OrderDto> MapToDtoWithItems(Order order, CancellationToken cancellationToken)
        {
            var orderItems = await _orderItemRepository.Finds(
                oi => oi.OrderId == order.Id,
                cancellationToken);

            var items = new List<OrderItemDto>();

            if (orderItems != null)
            {
                foreach (var item in orderItems)
                {
                    var dish = await _dishRepository.GetByID(item.DishId, cancellationToken);

                    items.Add(new OrderItemDto
                    {
                        Id = item.Id,
                        DishId = item.DishId,
                        DishName = dish?.Name ?? "Desconocido",
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Subtotal,
                        Notes = item.Notes
                    });
                }
            }

            return new OrderDto
            {
                Id = order.Id,
                TableNumber = order.TableNumber,
                Waiter = order.Waiter,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                StatusEnum = order.Status,
                Total = items.Sum(i => i.Subtotal),
                Items = items
            };
        }
    }
}
