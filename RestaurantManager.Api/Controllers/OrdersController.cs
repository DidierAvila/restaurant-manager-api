using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Api.Attributes;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Application.Features.Orders.Commands;
using RestaurantManager.Application.Features.Orders.Queries;

namespace RestaurantManager.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de pedidos del restaurante
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtener todos los pedidos con paginación y filtros opcionales
        /// </summary>
        [HttpGet]
        [RequirePermission("orders.read")]
        public async Task<ActionResult<PaginationResponseDto<OrderSummaryDto>>> GetAll(
            [FromQuery] OrderFilterDto filter,
            CancellationToken cancellationToken)
        {
            var query = new GetAllOrdersQuery { Filter = filter };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Obtener pedido por ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermission("orders.read")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var query = new GetOrderByIdQuery { Id = id };
            var order = await _mediator.Send(query);

            if (order == null)
                return NotFound(new { message = $"Pedido con ID {id} no encontrado" });

            return Ok(order);
        }

        /// <summary>
        /// Obtener detalle de items de un pedido por ID del pedido
        /// </summary>
        [HttpGet("{orderId}/items")]
        [RequirePermission("orders.read")]
        public async Task<ActionResult<List<OrderItemDto>>> GetItemsByOrderId(int orderId, CancellationToken cancellationToken)
        {
            var query = new GetOrderItemsByOrderIdQuery { OrderId = orderId };
            var items = await _mediator.Send(query, cancellationToken);

            if (items == null)
                return NotFound(new { message = $"Pedido con ID {orderId} no encontrado" });

            return Ok(items);
        }

        /// <summary>
        /// Obtener pedidos activos (no entregados ni cerrados)
        /// </summary>
        [HttpGet("active")]
        [RequirePermission("orders.read")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetActive()
        {
            var query = new GetActiveOrdersQuery();
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        /// <summary>
        /// Obtener pedido activo de una mesa
        /// </summary>
        [HttpGet("table/{tableNumber}")]
        [RequirePermission("orders.read")]
        public async Task<ActionResult<OrderDto>> GetByTable(int tableNumber)
        {
            var query = new GetOrderByTableQuery { TableNumber = tableNumber };
            var order = await _mediator.Send(query);

            if (order == null)
                return NotFound(new { message = $"No hay pedido activo en la mesa {tableNumber}" });

            return Ok(order);
        }

        /// <summary>
        /// Crear nuevo pedido
        /// </summary>
        [HttpPost]
        [RequirePermission("orders.create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
            }

            return HandleError(result.Error);
        }

        /// <summary>
        /// Agregar plato a un pedido
        /// </summary>
        [HttpPost("{orderId}/items")]
        [RequirePermission("orders.update")]
        public async Task<IActionResult> AddItem(int orderId, [FromBody] AddOrderItemDto dto)
        {
            var command = new AddOrderItemCommand
            {
                OrderId = orderId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                Notes = dto.Notes
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Quitar plato de un pedido
        /// </summary>
        [HttpDelete("{orderId}/items/{itemId}")]
        [RequirePermission("orders.update")]
        public async Task<IActionResult> RemoveItem(int orderId, int itemId)
        {
            var command = new RemoveOrderItemCommand
            {
                OrderId = orderId,
                OrderItemId = itemId
            };

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Avanzar estado del pedido (Abierto → En Preparación → Listo → Entregado → Cerrado)
        /// </summary>
        [HttpPatch("{id}/advance-status")]
        [RequirePermission("orders.update")]
        public async Task<IActionResult> AdvanceStatus(int id)
        {
            var command = new AdvanceOrderStatusCommand { OrderId = id };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
