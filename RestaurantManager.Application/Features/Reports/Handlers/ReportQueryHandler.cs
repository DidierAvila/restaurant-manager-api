using MediatR;
using RestaurantManager.Application.DTOs.Reports;
using RestaurantManager.Application.Features.Reports.Queries;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.Reports.Handlers
{
    public class ReportQueryHandler :
        IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
    {
        private readonly IRepositoryBase<Order> _orderRepository;
        private readonly IRepositoryBase<OrderItem> _orderItemRepository;
        private readonly IRepositoryBase<Dish> _dishRepository;

        public ReportQueryHandler(
            IRepositoryBase<Order> orderRepository,
            IRepositoryBase<OrderItem> orderItemRepository,
            IRepositoryBase<Dish> dishRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
        }

        public async Task<Result<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar fechas
                if (request.FromDate > request.ToDate)
                {
                    return Result.Failure<SalesReportDto>(Error.Validation("Report.InvalidDates", "La fecha 'desde' no puede ser mayor que 'hasta'"));
                }

            // Obtener pedidos cerrados o entregados en el rango de fechas
            var orders = await _orderRepository.Finds(
                o => (o.Status == OrderStatus.Entregado || o.Status == OrderStatus.Cerrado) &&
                     o.OrderDate >= request.FromDate &&
                     o.OrderDate < request.ToDate.AddDays(1),
                cancellationToken);

            if (orders == null || !orders.Any())
            {
                return new SalesReportDto
                {
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TotalOrders = 0,
                    TotalSales = 0,
                    AverageTicket = 0
                };
            }

            var orderIds = orders.Select(o => o.Id).ToList();

            // Obtener todos los items de esos pedidos
            var allOrderItems = await _orderItemRepository.GetAll(cancellationToken);
            var orderItems = allOrderItems.Where(oi => orderIds.Contains(oi.OrderId)).ToList();

            // Resumen general
            int totalOrders = orders.Count();
            decimal totalSales = orderItems.Sum(oi => oi.Subtotal);
            decimal averageTicket = totalOrders > 0 ? totalSales / totalOrders : 0;

            // Plato estrella (más vendido)
            var topDish = await GetTopDish(orderItems, cancellationToken);

            // Ventas por categoría
            var salesByCategory = await GetSalesByCategory(orderItems, totalSales, cancellationToken);

                // Ventas por plato
                var salesByDish = await GetSalesByDish(orderItems, cancellationToken);

                return Result.Success(new SalesReportDto
                {
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TotalOrders = totalOrders,
                    TotalSales = totalSales,
                    AverageTicket = averageTicket,
                    TopDish = topDish,
                    SalesByCategory = salesByCategory,
                    SalesByDish = salesByDish
                });
            }
            catch (Exception ex)
            {
                return Result.Failure<SalesReportDto>(Error.Failure("Report.Generate", ex.Message));
            }
        }

        private async Task<TopDishDto?> GetTopDish(List<OrderItem> orderItems, CancellationToken cancellationToken)
        {
            if (!orderItems.Any())
                return null;

            var dishGroups = orderItems
                .GroupBy(oi => oi.DishId)
                .Select(g => new
                {
                    DishId = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Subtotal)
                })
                .OrderByDescending(x => x.QuantitySold)
                .FirstOrDefault();

            if (dishGroups == null)
                return null;

            var dish = await _dishRepository.GetByID(dishGroups.DishId, cancellationToken);
            if (dish == null)
                return null;

            return new TopDishDto
            {
                Name = dish.Name,
                Category = dish.Category.ToString(),
                QuantitySold = dishGroups.QuantitySold,
                TotalRevenue = dishGroups.TotalRevenue
            };
        }

        private async Task<List<CategorySalesDto>> GetSalesByCategory(
            List<OrderItem> orderItems,
            decimal totalSales,
            CancellationToken cancellationToken)
        {
            if (!orderItems.Any())
                return new List<CategorySalesDto>();

            var allDishes = await _dishRepository.GetAll(cancellationToken);
            var dishDict = allDishes.ToDictionary(d => d.Id, d => d);

            var categoryGroups = orderItems
                .Where(oi => dishDict.ContainsKey(oi.DishId))
                .GroupBy(oi => dishDict[oi.DishId].Category)
                .Select(g => new CategorySalesDto
                {
                    Category = g.Key.ToString(),
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalSales = g.Sum(oi => oi.Subtotal),
                    Percentage = totalSales > 0 ? (g.Sum(oi => oi.Subtotal) * 100 / totalSales) : 0
                })
                .OrderByDescending(x => x.TotalSales)
                .ToList();

            return categoryGroups;
        }

        private async Task<List<DishSalesDto>> GetSalesByDish(
            List<OrderItem> orderItems,
            CancellationToken cancellationToken)
        {
            if (!orderItems.Any())
                return new List<DishSalesDto>();

            var allDishes = await _dishRepository.GetAll(cancellationToken);
            var dishDict = allDishes.ToDictionary(d => d.Id, d => d);

            var dishGroups = orderItems
                .Where(oi => dishDict.ContainsKey(oi.DishId))
                .GroupBy(oi => oi.DishId)
                .Select(g =>
                {
                    var dish = dishDict[g.Key];
                    return new DishSalesDto
                    {
                        DishName = dish.Name,
                        Category = dish.Category.ToString(),
                        QuantitySold = g.Sum(oi => oi.Quantity),
                        TotalSales = g.Sum(oi => oi.Subtotal)
                    };
                })
                .OrderByDescending(x => x.TotalSales)
                .ToList();

            return dishGroups;
        }
    }
}
