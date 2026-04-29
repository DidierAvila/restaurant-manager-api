using Microsoft.AspNetCore.Mvc;

namespace RestaurantManager.Api.Controllers
{
    /// <summary>
    /// Controlador para reportes de ventas del restaurante
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ReportsController : ControllerBase
    {
        // TODO: Implementar reportes de ventas
        // - Filtro por rango de fechas
        // - Resumen general (total ventas, cantidad pedidos, ticket promedio)
        // - Plato estrella (más vendido)
        // - Ventas por categoría con porcentajes
        // - Detalle por plato
    }
}
