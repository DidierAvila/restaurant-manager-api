using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Application.Features.Dishes.Commands;
using RestaurantManager.Application.Features.Dishes.Queries;

namespace RestaurantManager.Api.Controllers
{
    [Route("api/[controller]")]
    public class DishesController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public DishesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtener todos los platos con paginación y filtros opcionales
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginationResponseDto<DishDto>>> GetAll(
            [FromQuery] DishFilterDto filter,
            CancellationToken cancellationToken)
        {
            var query = new GetAllDishesQuery
            {
                Filter = filter
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Obtener plato por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DishDto>> GetById(int id)
        {
            var query = new GetDishByIdQuery { Id = id };
            var dish = await _mediator.Send(query);

            if (dish == null)
                return NotFound(new { message = $"Plato con ID {id} no encontrado" });

            return Ok(dish);
        }

        /// <summary>
        /// Obtener platos disponibles
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<List<DishDto>>> GetAvailable()
        {
            var query = new GetAvailableDishesQuery();
            var dishes = await _mediator.Send(query);
            return Ok(dishes);
        }

        /// <summary>
        /// Obtener platos por categoría
        /// </summary>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<DishDto>>> GetByCategory(int category)
        {
            var query = new GetDishesByCategoryQuery
            {
                Category = (Domain.Entities.DishCategory)category
            };

            var dishes = await _mediator.Send(query);
            return Ok(dishes);
        }

        /// <summary>
        /// Crear nuevo plato
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDishCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
            }

            return HandleError(result.Error);
        }

        /// <summary>
        /// Actualizar plato
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDishCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { message = "El ID del plato no coincide" });

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Eliminar plato
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDishCommand { Id = id };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Cambiar disponibilidad del plato (toggle)
        /// </summary>
        [HttpPatch("{id}/toggle-availability")]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            var command = new ToggleDishAvailabilityCommand { Id = id };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
