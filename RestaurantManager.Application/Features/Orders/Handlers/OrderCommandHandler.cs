using MediatR;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Application.Features.Orders.Commands;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Errors;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.Orders.Handlers
{
    public class OrderCommandHandler :
        IRequestHandler<CreateOrderCommand, Result<OrderDto>>,
        IRequestHandler<AddOrderItemCommand, Result<OrderDto>>,
        IRequestHandler<RemoveOrderItemCommand, Result<OrderDto>>,
        IRequestHandler<AdvanceOrderStatusCommand, Result<OrderDto>>
    {
        private readonly IRepositoryBase<Order> _orderRepository;
        private readonly IRepositoryBase<OrderItem> _orderItemRepository;
        private readonly IRepositoryBase<Dish> _dishRepository;

        public OrderCommandHandler(
            IRepositoryBase<Order> orderRepository,
            IRepositoryBase<OrderItem> orderItemRepository,
            IRepositoryBase<Dish> dishRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _dishRepository = dishRepository;
        }

        public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validar número de mesa
            if (request.TableNumber <= 0)
            {
                return Result.Failure<OrderDto>(OrderErrors.InvalidTableNumber());
            }

            // Validar mesero
            if (string.IsNullOrWhiteSpace(request.Waiter))
            {
                return Result.Failure<OrderDto>(OrderErrors.InvalidWaiter());
            }

            // Verificar que no haya un pedido abierto en esa mesa
            var existingOrders = await _orderRepository.Finds(
                o => o.TableNumber == request.TableNumber &&
                     o.Status != OrderStatus.Entregado &&
                     o.Status != OrderStatus.Cerrado,
                cancellationToken);

            if (existingOrders != null && existingOrders.Any())
            {
                return Result.Failure<OrderDto>(OrderErrors.TableAlreadyHasActiveOrder(request.TableNumber));
            }

            var order = new Order
            {
                TableNumber = request.TableNumber,
                Waiter = request.Waiter,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Abierto
            };

            var createdOrder = await _orderRepository.Create(order, cancellationToken);
            var orderDto = await MapToDtoWithItems(createdOrder, cancellationToken);

            return Result.Success(orderDto);
        }

        public async Task<Result<OrderDto>> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            // Validar cantidad
            if (request.Quantity <= 0)
            {
                return Result.Failure<OrderDto>(OrderErrors.InvalidQuantity());
            }

            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                return Result.Failure<OrderDto>(OrderErrors.NotFound(request.OrderId));
            }

            // Verificar que el pedido siga abierto
            if (order.Status != OrderStatus.Abierto)
            {
                return Result.Failure<OrderDto>(OrderErrors.CannotEditClosedOrder());
            }

            // Verificar que el plato exista y esté disponible
            var dish = await _dishRepository.GetByID(request.DishId, cancellationToken);
            if (dish == null)
            {
                return Result.Failure<OrderDto>(DishErrors.NotFound(request.DishId));
            }

            if (!dish.IsAvailable)
            {
                return Result.Failure<OrderDto>(OrderErrors.DishNotAvailable(dish.Name));
            }

            // Verificar si ya existe ese plato en el pedido
            var existingItem = await _orderItemRepository.Find(
                oi => oi.OrderId == request.OrderId && oi.DishId == request.DishId,
                cancellationToken);

            if (existingItem != null)
            {
                // Ya existe: sumar cantidad
                existingItem.Quantity += request.Quantity;
                await _orderItemRepository.Update(existingItem, cancellationToken);
            }
            else
            {
                // Insertar nuevo detalle
                var orderItem = new OrderItem
                {
                    OrderId = request.OrderId,
                    DishId = request.DishId,
                    Quantity = request.Quantity,
                    UnitPrice = dish.Price,
                    Notes = request.Notes
                };

                await _orderItemRepository.Create(orderItem, cancellationToken);
            }

            var orderDto = await MapToDtoWithItems(order, cancellationToken);
            return Result.Success(orderDto);
        }

        public async Task<Result<OrderDto>> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                return Result.Failure<OrderDto>(OrderErrors.NotFound(request.OrderId));
            }

            if (order.Status != OrderStatus.Abierto)
            {
                return Result.Failure<OrderDto>(OrderErrors.CannotEditClosedOrder());
            }

            var orderItem = await _orderItemRepository.GetByID(request.OrderItemId, cancellationToken);
            if (orderItem == null || orderItem.OrderId != request.OrderId)
            {
                return Result.Failure<OrderDto>(OrderErrors.OrderItemNotFound(request.OrderItemId));
            }

            await _orderItemRepository.Delete(orderItem, cancellationToken);
            var orderDto = await MapToDtoWithItems(order, cancellationToken);

            return Result.Success(orderDto);
        }

        public async Task<Result<OrderDto>> Handle(AdvanceOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                return Result.Failure<OrderDto>(OrderErrors.NotFound(request.OrderId));
            }

            // Máquina de estados
            var newStatus = order.Status switch
            {
                OrderStatus.Abierto => OrderStatus.EnPreparacion,
                OrderStatus.EnPreparacion => OrderStatus.Listo,
                OrderStatus.Listo => OrderStatus.Entregado,
                OrderStatus.Entregado => OrderStatus.Cerrado,
                _ => (OrderStatus?)null
            };

            if (newStatus == null)
            {
                return Result.Failure<OrderDto>(OrderErrors.CannotAdvanceStatus());
            }

            // Si está en Abierto, verificar que tenga platos
            if (order.Status == OrderStatus.Abierto)
            {
                var items = await _orderItemRepository.Finds(
                    oi => oi.OrderId == request.OrderId,
                    cancellationToken);

                if (items == null || !items.Any())
                {
                    return Result.Failure<OrderDto>(OrderErrors.OrderHasNoItems());
                }
            }

            order.Status = newStatus.Value;
            await _orderRepository.Update(order, cancellationToken);
            var orderDto = await MapToDtoWithItems(order, cancellationToken);

            return Result.Success(orderDto);
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
