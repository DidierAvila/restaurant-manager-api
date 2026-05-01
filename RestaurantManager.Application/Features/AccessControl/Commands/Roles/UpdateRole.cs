using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;
using RestaurantManager.Application.Features.AccessControl.Queries.Permissions;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Roles;

public class UpdateRole
{
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly GetPermissionsForDropdown _getPermissionsForDropdown;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateRole> _logger;

    public UpdateRole(
        IRepositoryBase<Role> roleRepository, 
        IRepositoryBase<Permission> permissionRepository, 
        IRolePermissionRepository rolePermissionRepository, 
        GetPermissionsForDropdown getPermissionsForDropdown,
        IMapper mapper,
        ILogger<UpdateRole> logger)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _getPermissionsForDropdown = getPermissionsForDropdown;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RoleDto>> HandleAsync(Guid id, UpdateRoleDto updateRoleDto, CancellationToken cancellationToken)
    {
        // Find existing role
        var role = await _roleRepository.Find(x => x.Id == id, cancellationToken);
        if (role == null)
            return Result.Failure<RoleDto>(Error.NotFound("Role.NotFound", "Role not found"));

        // Check if name already exists (excluding current role)
        if (!string.IsNullOrWhiteSpace(updateRoleDto.Name))
        {
            var existingRole = await _roleRepository.Find(x => x.Name == updateRoleDto.Name && x.Id != id, cancellationToken);
            if (existingRole != null)
                return Result.Failure<RoleDto>(Error.Conflict("Role.NameExists", "Role with this name already exists"));
        }

        // Map DTO properties to existing entity using AutoMapper
        _mapper.Map(updateRoleDto, role);

        // Ensure UpdatedAt is set
        role.UpdatedAt = DateTime.Now;

        // Update in repository
        await _roleRepository.Update(role, cancellationToken);

        // Actualizar permisos si se proporcionaron
        if (updateRoleDto.PermissionIds != null)
        {
            await UpdateRolePermissions(id, updateRoleDto.PermissionIds, cancellationToken);
        }

        // Map Entity to DTO using AutoMapper
        var roleDto = _mapper.Map<RoleDto>(role);

        // Cargar permisos actuales para incluir en la respuesta
        var permissions = await _getPermissionsForDropdown.HandleAsync(id, cancellationToken);
        roleDto.Permissions = [.. permissions];

        return Result.Success(roleDto);
    }

    /// <summary>
    /// Actualiza los permisos de un rol (reemplaza los existentes)
    /// </summary>
    private async Task UpdateRolePermissions(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Iniciando actualización de permisos para el rol {RoleId}", roleId);

            // Obtener permisos actuales del rol
            var currentRolePermissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId, cancellationToken);
            var currentPermissionIds = currentRolePermissions.Select(rp => rp.PermissionId).ToList();

            // Verificar si hay cambios en los permisos
            var newPermissionIds = permissionIds ?? new List<Guid>();
            var hasChanges = !currentPermissionIds.OrderBy(x => x).SequenceEqual(newPermissionIds.OrderBy(x => x));

            if (!hasChanges)
            {
                _logger.LogInformation("No hay cambios en los permisos del rol {RoleId}, omitiendo invalidación de sesiones", roleId);
                return;
            }

            // Eliminar permisos actuales
            foreach (var currentRolePermission in currentRolePermissions)
            {
                await _rolePermissionRepository.Delete(currentRolePermission, cancellationToken);
            }

            // Asignar nuevos permisos
            foreach (var permissionId in newPermissionIds)
            {
                var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                if (permission != null)
                {
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    };

                    await _rolePermissionRepository.Create(rolePermission, cancellationToken);
                }
            }

            _logger.LogInformation("Permisos actualizados exitosamente para el rol {RoleId}. Invalidando sesiones de usuarios con este rol", roleId);

            //// Invalidar sesiones de usuarios que tienen este rol
            //var invalidatedSessions = await _sessionInvalidationService.InvalidateSessionsByRoleAsync(roleId, cancellationToken);
            
            //_logger.LogInformation("Actualización de permisos completada para el rol {RoleId}. {InvalidatedSessions} sesiones invalidadas", 
            //    roleId, invalidatedSessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar permisos del rol {RoleId}", roleId);
            throw;
        }
    }

    private async Task<List<PermissionDto>> LoadRolePermissions(Guid roleId, CancellationToken cancellationToken)
    {
        var rolePermissionRelations = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId, cancellationToken);
        var permissionIds = rolePermissionRelations.Select(rp => rp.PermissionId).ToList();

        var permissions = new List<Permission>();
        foreach (var permissionId in permissionIds)
        {
            var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
            if (permission != null)
            {
                permissions.Add(permission);
            }
        }

        return _mapper.Map<List<PermissionDto>>(permissions);
    }
}
