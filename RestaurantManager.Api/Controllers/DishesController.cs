using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.DTOs.Dishes;
using RestaurantManager.Application.Features.Dishes.Commands;
using RestaurantManager.Application.Features.Dishes.Queries;

namespace RestaurantManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
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
        public async Task<ActionResult<DishDto>> Create([FromBody] CreateDishCommand command)
        {
            try
            {
                var dish = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = dish.Id }, dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizar plato
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DishDto>> Update(int id, [FromBody] UpdateDishCommand command)
        {
            try
            {
                if (id != command.Id)
                    return BadRequest(new { message = "El ID del plato no coincide" });

                var dish = await _mediator.Send(command);
                return Ok(dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Eliminar plato
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteDishCommand { Id = id };
                var result = await _mediator.Send(command);
                return Ok(new { message = "Plato eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cambiar disponibilidad del plato (toggle)
        /// </summary>
        [HttpPatch("{id}/toggle-availability")]
        public async Task<ActionResult<DishDto>> ToggleAvailability(int id)
        {
            try
            {
                var command = new ToggleDishAvailabilityCommand { Id = id };
                var dish = await _mediator.Send(command);
                return Ok(dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
