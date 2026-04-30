using MediatR;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Application.Features.Orders.Commands;
using RestaurantManager.Domain.Entities;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.Orders.Handlers
{
    public class OrderCommandHandler :
        IRequestHandler<CreateOrderCommand, OrderDto>,
        IRequestHandler<AddOrderItemCommand, OrderDto>,
        IRequestHandler<RemoveOrderItemCommand, OrderDto>,
        IRequestHandler<AdvanceOrderStatusCommand, OrderDto>
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

        public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validar número de mesa
            if (request.TableNumber <= 0 || request.TableNumber > 50)
            {
                throw new Exception("Número de mesa inválido (1-50)");
            }

            // Validar mesero
            if (string.IsNullOrWhiteSpace(request.Waiter))
            {
                throw new Exception("El nombre del mesero es obligatorio");
            }

            // Verificar que no haya un pedido abierto en esa mesa
            var existingOrders = await _orderRepository.Finds(
                o => o.TableNumber == request.TableNumber &&
                     o.Status != OrderStatus.Entregado &&
                     o.Status != OrderStatus.Cerrado,
                cancellationToken);

            if (existingOrders != null && existingOrders.Any())
            {
                throw new Exception($"Ya hay un pedido abierto en la mesa {request.TableNumber}. Ciérralo primero.");
            }

            var order = new Order
            {
                TableNumber = request.TableNumber,
                Waiter = request.Waiter,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Abierto
            };

            var createdOrder = await _orderRepository.Create(order, cancellationToken);

            return await MapToDtoWithItems(createdOrder, cancellationToken);
        }

        public async Task<OrderDto> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
        {
            // Validar cantidad
            if (request.Quantity <= 0 || request.Quantity > 20)
            {
                throw new Exception("Cantidad inválida (1-20)");
            }

            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Pedido con ID {request.OrderId} no encontrado");
            }

            // Verificar que el pedido siga abierto
            if (order.Status != OrderStatus.Abierto)
            {
                throw new Exception("Solo se pueden agregar platos a pedidos en estado 'Abierto'");
            }

            // Verificar que el plato exista y esté disponible
            var dish = await _dishRepository.GetByID(request.DishId, cancellationToken);
            if (dish == null)
            {
                throw new Exception($"Plato con ID {request.DishId} no encontrado");
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

            return await MapToDtoWithItems(order, cancellationToken);
        }

        public async Task<OrderDto> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Pedido con ID {request.OrderId} no encontrado");
            }

            var orderItem = await _orderItemRepository.GetByID(request.OrderItemId, cancellationToken);
            if (orderItem == null || orderItem.OrderId != request.OrderId)
            {
                throw new Exception($"Item de pedido con ID {request.OrderItemId} no encontrado");
            }

            await _orderItemRepository.Delete(orderItem, cancellationToken);

            return await MapToDtoWithItems(order, cancellationToken);
        }

        public async Task<OrderDto> Handle(AdvanceOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByID(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Pedido con ID {request.OrderId} no encontrado");
            }

            // Máquina de estados
            var newStatus = order.Status switch
            {
                OrderStatus.Abierto => OrderStatus.EnPreparacion,
                OrderStatus.EnPreparacion => OrderStatus.Listo,
                OrderStatus.Listo => OrderStatus.Entregado,
                OrderStatus.Entregado => OrderStatus.Cerrado,
                _ => throw new Exception($"El pedido ya está cerrado")
            };

            // Si está en Abierto, verificar que tenga platos
            if (order.Status == OrderStatus.Abierto)
            {
                var items = await _orderItemRepository.Finds(
                    oi => oi.OrderId == request.OrderId,
                    cancellationToken);

                if (items == null || !items.Any())
                {
                    throw new Exception("No se puede avanzar un pedido sin platos");
                }
            }

            order.Status = newStatus;
            await _orderRepository.Update(order, cancellationToken);

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
