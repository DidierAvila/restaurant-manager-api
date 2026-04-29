using RestaurantManager.Application.Core.Auth.Commands.RolePermissions;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Roles;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public class RoleCommandHandler : IRoleCommandHandler
{
    private readonly CreateRole _createRole;
    private readonly UpdateRole _updateRole;
    private readonly DeleteRole _deleteRole;
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RoleCommandHandler(CreateRole createRole, UpdateRole updateRole, DeleteRole deleteRole, IRolePermissionRepository rolePermissionRepository)
    {
        _createRole = createRole;
        _updateRole = updateRole;
        _deleteRole = deleteRole;
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<RoleDto> CreateRole(CreateRoleDto command, CancellationToken cancellationToken)
    {
        return await _createRole.HandleAsync(command, cancellationToken);
    }

    public async Task<RoleDto> UpdateRole(Guid id, UpdateRoleDto command, CancellationToken cancellationToken)
    {
        return await _updateRole.HandleAsync(id, command, cancellationToken);
    }

    public async Task<bool> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        return await _deleteRole.HandleAsync(id, cancellationToken);
    }

    /// <summary>
    /// Remover múltiples permisos de un rol
    /// </summary>
    public async Task<MultiplePermissionRemovalResult> RemoveMultiplePermissionsFromRole(RemoveMultiplePermissionsFromRole command, CancellationToken cancellationToken)
    {
        var result = new MultiplePermissionRemovalResult();

        foreach (var permissionId in command.PermissionIds)
        {
            try
            {
                // Verificar si el permiso está asignado al rol
                var rolePermissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(command.RoleId, cancellationToken);
                var hasPermission = rolePermissions.Any(rp => rp.PermissionId == permissionId);

                if (!hasPermission)
                {
                    result.NotAssignedPermissions.Add(permissionId);
                    continue;
                }

                // Remover el permiso del rol
                await _rolePermissionRepository.RemovePermissionFromRoleAsync(command.RoleId, permissionId, cancellationToken);
                result.RemovedPermissions.Add(permissionId);
            }
            catch (Exception ex)
            {
                result.FailedPermissions.Add($"Error removing permission {permissionId}: {ex.Message}");
            }
        }

        return result;
    }
}
