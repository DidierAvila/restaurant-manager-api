using Microsoft.AspNetCore.Mvc;

namespace RestaurantManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint para verificar que la API está funcionando
        /// </summary>
        /// <returns>Estado de la aplicación</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Service = "Restaurant Manager API"
            });
        }

        /// <summary>
        /// Endpoint de prueba que devuelve un mensaje
        /// </summary>
        /// <param name="name">Nombre a saludar</param>
        /// <returns>Mensaje de saludo</returns>
        [HttpGet("greet/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Greet(string name)
        {
            return Ok(new
            {
                Message = $"Hola {name}, bienvenido a Restaurant Manager API",
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
