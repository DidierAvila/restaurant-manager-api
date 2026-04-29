using Microsoft.AspNetCore.Mvc;

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
        // TODO: Implementar gestión de pedidos
        // - Crear pedido (asociado a mesa y mesero)
        // - Agregar platos al pedido
        // - Cambiar estado del pedido (Abierto → En Preparación → Listo → Entregado → Cerrado)
        // - Calcular total del pedido
        // - Validar reglas de negocio (no dos pedidos en misma mesa, etc.)
    }
}
