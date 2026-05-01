using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Api.Attributes;
using RestaurantManager.Application.Core.Auth.Queries.RolePermissions;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Commands.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

namespace RestaurantManager.Api.Controllers.AccessControl;

/// <summary>
/// Controlador para gestionar los permisos del sistema.
/// </summary>
[Route("Api/Auth/[controller]")]
[Authorize]
public class PermissionsController : ApiControllerBase
{
    private readonly IPermissionCommandHandler _commandHandler;
    private readonly IPermissionQueryHandler _queryHandler;
    private readonly GetRolesByPermission _getRolesByPermission;

    /// <summary>
    /// Constructor del controlador PermissionsController
    /// </summary>
    /// <param name="commandHandler"></param>
    /// <param name="queryHandler"></param>
    /// <param name="getRolesByPermission"></param>
    public PermissionsController(
        IPermissionCommandHandler commandHandler,
        IPermissionQueryHandler queryHandler,
        GetRolesByPermission getRolesByPermission)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
        _getRolesByPermission = getRolesByPermission;
    }

    /// <summary>
    /// Create a new Permission
    /// </summary>
    [HttpPost]
    [RequirePermission("permissions.create")]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionDto command, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.CreatePermission(command, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetPermissionById), new { id = result.Value.Id }, result.Value);
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Update an existing Permission
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermission("permissions.update")]
    public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionDto command, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.UpdatePermission(id, command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a Permission
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermission("permissions.delete")]
    public async Task<IActionResult> DeletePermission(Guid id, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.DeletePermission(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get Permission by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequirePermission("permissions.read")]
    public async Task<IActionResult> GetPermissionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetPermissionById(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene una lista paginada de permisos con filtros opcionales
    /// </summary>
    /// <param name="filter">Filtros de búsqueda y paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de permisos</returns>
    /// <remarks>
    /// Campos disponibles para SortBy: name, description, status, createdat
    ///
    /// Ejemplo de uso:
    /// GET /api/permissions?page=1&amp;pageSize=10&amp;name=read&amp;status=true&amp;sortBy=name
    /// </remarks>
    [HttpGet]
    [RequirePermission("permissions.read")]
    public async Task<IActionResult> GetAllPermissions(
        [FromQuery] PermissionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetAllPermissionsFiltered(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all Permissions (simple list without pagination)
    /// </summary>
    [HttpGet("simple")]
    [RequirePermission("permissions.read")]
    public async Task<IActionResult> GetAllPermissionsSimple(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetAllPermissions(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get active Permissions only
    /// </summary>
    [HttpGet("active")]
    [RequirePermission("permissions.read")]
    public async Task<IActionResult> GetActivePermissions(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetActivePermissions(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene lista optimizada de permisos para componentes dropdown del frontend
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista simplificada de permisos activos con solo Id y Name, ordenada alfabéticamente</returns>
    /// <response code="200">Lista de permisos para dropdown obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    /// <remarks>
    /// Este endpoint está optimizado para cargar listas desplegables en el frontend.
    /// Solo retorna permisos activos con los campos esenciales (Id, Name) ordenados alfabéticamente.
    ///
    /// Ejemplo de respuesta:
    /// [
    ///   { "id": "123e4567-e89b-12d3-a456-426614174000", "name": "Create Users" },
    ///   { "id": "123e4567-e89b-12d3-a456-426614174001", "name": "Delete Users" },
    ///   { "id": "123e4567-e89b-12d3-a456-426614174002", "name": "Read Users" }
    /// ]
    /// </remarks>
    [HttpGet("dropdown")]
    [RequirePermission("permissions.read")]
    [ProducesResponseType(typeof(IEnumerable<PermissionDropdownDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetPermissionsForDropdown(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetPermissionsForDropdown(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get Permissions summary with role count
    /// </summary>
    [HttpGet("summary")]
    [RequirePermission("permissions.read")]
    public async Task<IActionResult> GetPermissionsSummary(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.GetPermissionsSummary(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtener todos los roles que tienen un permiso específico
    /// </summary>
    [HttpGet("{permissionId}/roles")]
    [RequirePermission("permissions.read")]
    public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetPermissionRoles(Guid permissionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getRolesByPermission.HandleAsync(permissionId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
