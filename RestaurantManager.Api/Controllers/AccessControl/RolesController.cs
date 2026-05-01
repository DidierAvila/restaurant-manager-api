using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Api.Attributes;
using RestaurantManager.Application.Core.Auth.Commands.RolePermissions;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Commands.Handlers;
using RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.RolePermissions;

namespace RestaurantManager.Api.Controllers.AccessControl;

/// <summary>
/// Controlador para gestionar roles y sus permisos.
/// </summary>
/// <param name="roleCommandHandler"></param>
/// <param name="roleQueryHandler"></param>
/// <param name="getPermissionsByRole"></param>
/// <param name="assignPermissionToRole"></param>
/// <param name="assignMultiplePermissionsToRole"></param>
/// <param name="removePermissionFromRole"></param>
[Route("Api/Auth/[controller]")]
[Authorize]
public class RolesController(
    IRoleCommandHandler roleCommandHandler,
    IRoleQueryHandler roleQueryHandler,
    GetPermissionsByRole getPermissionsByRole,
    AssignPermissionToRole assignPermissionToRole,
    AssignMultiplePermissionsToRole assignMultiplePermissionsToRole,
    RemovePermissionFromRole removePermissionFromRole) : ApiControllerBase
{
    private readonly IRoleCommandHandler _roleCommandHandler = roleCommandHandler;
    private readonly IRoleQueryHandler _roleQueryHandler = roleQueryHandler;
    private readonly GetPermissionsByRole _getPermissionsByRole = getPermissionsByRole;
    private readonly AssignPermissionToRole _assignPermissionToRole = assignPermissionToRole;
    private readonly AssignMultiplePermissionsToRole _assignMultiplePermissionsToRole = assignMultiplePermissionsToRole;
    private readonly RemovePermissionFromRole _removePermissionFromRole = removePermissionFromRole;

    /// <summary>
    /// Obtiene una lista paginada de roles con filtros opcionales
    /// </summary>
    /// <param name="filter">Filtros de búsqueda y paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de roles</returns>
    /// <remarks>
    /// Campos disponibles para SortBy: name, description, status, createdat
    ///
    /// Ejemplo de uso:
    /// GET /api/roles?page=1&amp;pageSize=10&amp;name=admin&amp;status=true&amp;sortBy=name
    /// </remarks>
    [HttpGet]
    [RequirePermission("roles.read")]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] RoleFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _roleQueryHandler.GetAllRolesFiltered(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all Roles (simple list without pagination)
    /// </summary>
    [HttpGet("simple")]
    [RequirePermission("roles.read")]
    public async Task<IActionResult> GetAllRolesSimple(CancellationToken cancellationToken)
    {
        var result = await _roleQueryHandler.GetAllRoles(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene lista optimizada de roles para dropdowns/listas desplegables
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de roles activos con solo ID y nombre, ordenada alfabéticamente</returns>
    /// <remarks>
    /// Endpoint optimizado para componentes UI que requieren listas de roles:
    /// - Solo roles activos (status = true)
    /// - Solo campos ID y Name para máximo rendimiento
    /// - Ordenamiento alfabético automático por nombre
    /// - Sin paginación (lista completa)
    /// - Ideal para: Select, Dropdown, Multiselect, etc.
    ///
    /// Ejemplo de respuesta:
    /// [
    ///   { "id": "uuid-1", "name": "Administrador" },
    ///   { "id": "uuid-2", "name": "Empleado" },
    ///   { "id": "uuid-3", "name": "Manager de Ventas" }
    /// ]
    /// </remarks>
    [HttpGet("dropdown")]
    [RequirePermission("roles.read")]
    public async Task<IActionResult> GetRolesDropdown(CancellationToken cancellationToken)
    {
        var result = await _roleQueryHandler.GetRolesDropdown(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un rol por ID incluyendo sus permisos asociados
    /// </summary>
    /// <param name="id">ID del rol</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Rol con sus permisos asociados</returns>
    [HttpGet("{id}")]
    [RequirePermission("roles.read")]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roleQueryHandler.GetRoleById(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo rol con permisos opcionales
    /// </summary>
    /// <param name="createRoleDto">Datos del rol a crear, incluyendo permisos opcionales</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Rol creado con permisos asignados</returns>
    /// <remarks>
    /// Ejemplo de request body:
    /// {
    ///   "name": "Manager de Ventas",
    ///   "description": "Gestiona el equipo de ventas",
    ///   "status": true,
    ///   "permissionIds": ["uuid-perm-1", "uuid-perm-2"]
    /// }
    ///
    /// Si no se especifican permisos (permissionIds), el rol se creará sin permisos asignados.
    /// Los permisos se pueden asignar posteriormente usando los endpoints de gestión de permisos.
    /// </remarks>
    [HttpPost]
    [RequirePermission("roles.create")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto, CancellationToken cancellationToken)
    {
        var result = await _roleCommandHandler.CreateRole(createRoleDto, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetRoleById), new { id = result.Value.Id }, result.Value);
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Actualiza un rol existente incluyendo sus permisos
    /// </summary>
    /// <param name="id">ID del rol a actualizar</param>
    /// <param name="updateRoleDto">Datos a actualizar del rol</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Rol actualizado con permisos asignados</returns>
    /// <remarks>
    /// La respuesta incluye los permisos actuales del rol después de la actualización.
    /// Si se envían permissionIds, se reemplazarán todos los permisos existentes.
    /// Para gestionar permisos específicamente, usar los endpoints de gestión de permisos:
    /// - POST /api/auth/roles/{id}/permissions - Asignar permisos
    /// - DELETE /api/auth/roles/{id}/permissions - Remover permisos
    ///
    /// Ejemplo de request:
    /// {
    ///   "name": "Manager de Ventas Senior",
    ///   "description": "Gestiona el equipo de ventas con más privilegios",
    ///   "permissionIds": ["uuid-perm-1", "uuid-perm-2", "uuid-perm-3"]
    /// }
    ///
    /// Respuesta incluye permisos actualizados:
    /// {
    ///   "id": "uuid-rol",
    ///   "name": "Manager de Ventas Senior",
    ///   "permissions": [...permisos actualizados...]
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [RequirePermission("roles.update")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto updateRoleDto, CancellationToken cancellationToken)
    {
        var result = await _roleCommandHandler.UpdateRole(id, updateRoleDto, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un rol por ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [RequirePermission("roles.delete")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var result = await _roleCommandHandler.DeleteRole(id, cancellationToken);
        return HandleResult(result);
    }

    // GET: api/Roles/{id}/permissions
    /// <summary>
    /// Obtener todos los permisos asignados a un rol específico
    /// </summary>
    [HttpGet("{roleId}/permissions")]
    [RequirePermission("roles.read")]
    public async Task<ActionResult<IEnumerable<RolePermissionDto>>> GetRolePermissions(Guid roleId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _getPermissionsByRole.HandleAsync(roleId, cancellationToken);
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

    // POST: api/Roles/{roleId}/permissions/{permissionId}
    /// <summary>
    /// Asignar un permiso específico a un rol
    /// </summary>
    [HttpPost("{roleId}/permissions/{permissionId}")]
    [RequirePermission("roles.manage")]
    public async Task<ActionResult<RolePermissionDto>> AssignPermissionToRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        try
        {
            var request = new AssignPermissionToRoleDto { RoleId = roleId, PermissionId = permissionId };
            var result = await _assignPermissionToRole.HandleAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    // POST: api/Roles/{roleId}/permissions/bulk
    /// <summary>
    /// Asignar múltiples permisos a un rol de una sola vez
    /// Mejora la experiencia de usuario al permitir asignaciones masivas
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionIds">DTO con lista de IDs de permisos a asignar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado detallado de la asignación múltiple</returns>
    [HttpPost("{roleId}/permissions/bulk")]
    [RequirePermission("roles.manage")]
    public async Task<IActionResult> AssignMultiplePermissionsToRole(
        Guid roleId,
        [FromBody] List<Guid> permissionIds,
        CancellationToken cancellationToken)
    {
        var request = new AssignMultiplePermissionsToRoleDto
        {
            RoleId = roleId,
            PermissionIds = permissionIds
        };

        var result = await _assignMultiplePermissionsToRole.HandleAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandleError(result.Error);
        }

        // Si hay errores parciales en la asignación
        if (result.Value.HasErrors)
        {
            return BadRequest(new
            {
                message = "La asignación se completó con algunos errores",
                result = result.Value
            });
        }

        return Ok(new
        {
            message = "Permisos asignados exitosamente",
            result = result.Value
        });
    }

    // DELETE: api/Roles/{roleId}/permissions/{permissionId}
    /// <summary>
    /// Remover un permiso específico de un rol
    /// </summary>
    [HttpDelete("{roleId}/permissions/{permissionId}")]
    [RequirePermission("roles.manage")]
    public async Task<ActionResult<bool>> RemovePermissionFromRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _removePermissionFromRole.HandleAsync(roleId, permissionId, cancellationToken);
            return Ok(new { success = result, message = "Permiso removido del rol correctamente" });
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

    /// <summary>
    /// Remueve múltiples permisos de un rol
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    /// <param name="permissionIds">Lista de IDs de permisos a remover</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la remoción múltiple</returns>
    [HttpDelete("{roleId}/permissions")]
    [RequirePermission("roles.manage")]
    public async Task<IActionResult> RemoveMultiplePermissionsFromRole(
        Guid roleId,
        [FromBody] List<Guid> permissionIds,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMultiplePermissionsFromRole
        {
            RoleId = roleId,
            PermissionIds = permissionIds
        };

        var result = await _roleCommandHandler.RemoveMultiplePermissionsFromRole(command, cancellationToken);
        return HandleResult(result);
    }
}
