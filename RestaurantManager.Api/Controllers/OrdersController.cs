using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Orders;
using RestaurantManager.Application.Features.Orders.Commands;
using RestaurantManager.Application.Features.Orders.Queries;

namespace RestaurantManager.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de pedidos del restaurante
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
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
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var query = new GetOrderByIdQuery { Id = id };
            var order = await _mediator.Send(query);

            if (order == null)
                return NotFound(new { message = $"Pedido con ID {id} no encontrado" });

            return Ok(order);
        }

        /// <summary>
        /// Obtener pedidos activos (no entregados ni cerrados)
        /// </summary>
        [HttpGet("active")]
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
        public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderCommand command)
        {
            try
            {
                var order = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Agregar plato a un pedido
        /// </summary>
        [HttpPost("{orderId}/items")]
        public async Task<ActionResult<OrderDto>> AddItem(int orderId, [FromBody] AddOrderItemDto dto)
        {
            try
            {
                var command = new AddOrderItemCommand
                {
                    OrderId = orderId,
                    DishId = dto.DishId,
                    Quantity = dto.Quantity,
                    Notes = dto.Notes
                };

                var order = await _mediator.Send(command);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Quitar plato de un pedido
        /// </summary>
        [HttpDelete("{orderId}/items/{itemId}")]
        public async Task<ActionResult<OrderDto>> RemoveItem(int orderId, int itemId)
        {
            try
            {
                var command = new RemoveOrderItemCommand
                {
                    OrderId = orderId,
                    OrderItemId = itemId
                };

                var order = await _mediator.Send(command);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Avanzar estado del pedido (Abierto → En Preparación → Listo → Entregado → Cerrado)
        /// </summary>
        [HttpPatch("{id}/advance-status")]
        public async Task<ActionResult<OrderDto>> AdvanceStatus(int id)
        {
            try
            {
                var command = new AdvanceOrderStatusCommand { OrderId = id };
                var order = await _mediator.Send(command);
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
