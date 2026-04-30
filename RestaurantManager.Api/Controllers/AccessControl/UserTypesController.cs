using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Api.Attributes;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

namespace RestaurantManager.Api.Controllers.AccessControl;

/// <summary>
/// Controlador para gestionar los tipos de usuario.
/// </summary>
[ApiController]
[Route("Api/Auth/[controller]")]
[Authorize]
public class UserTypesController : ControllerBase
{
    private readonly IUserTypeQueryHandler _queryHandler;
    private readonly ILogger<UserTypesController> _logger;

    /// <summary>
    /// Constructor del controlador UserTypesController
    /// </summary>
    /// <param name="commandHandler"></param>
    /// <param name="queryHandler"></param>
    /// <param name="logger"></param>
    public UserTypesController(
        IUserTypeQueryHandler queryHandler,
        ILogger<UserTypesController> logger)
    {
        _queryHandler = queryHandler;
        _logger = logger;
    }


    /// <summary>
    /// Obtiene lista optimizada de tipos de usuario para dropdowns/listas desplegables
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de tipos de usuario activos con solo ID y nombre, ordenada alfabéticamente</returns>
    /// <remarks>
    /// Endpoint optimizado para componentes UI que requieren listas de tipos de usuario:
    /// - Solo tipos de usuario activos (status = true)
    /// - Solo campos ID y Name para máximo rendimiento  
    /// - Ordenamiento alfabético automático por nombre
    /// - Sin paginación (lista completa)
    /// - Ideal para: Select, Dropdown, Multiselect, etc.
    /// 
    /// Ejemplo de respuesta:
    /// [
    ///   { "id": "uuid-1", "name": "Administrador" },
    ///   { "id": "uuid-2", "name": "Cliente" },
    ///   { "id": "uuid-3", "name": "Empleado" }
    /// ]
    /// </remarks>
    [HttpGet("dropdown")]
    [RequirePermission("user_types.read")]
    public async Task<ActionResult<IEnumerable<UserTypeDropdownDto>>> GetUserTypesDropdown(CancellationToken cancellationToken)
    {
        var userTypes = await _queryHandler.GetUserTypesForDropdown(cancellationToken);
        return Ok(userTypes);
    }
}
