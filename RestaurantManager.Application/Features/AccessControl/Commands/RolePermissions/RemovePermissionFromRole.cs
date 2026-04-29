using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;

public class RemovePermissionFromRole
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RemovePermissionFromRole(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<bool> HandleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(roleId, permissionId, cancellationToken);
        if (rolePermission == null)
        {
            throw new KeyNotFoundException($"No se encontró la asignación del permiso al rol especificado");
        }

        await _rolePermissionRepository.RemovePermissionFromRoleAsync(roleId, permissionId, cancellationToken);
        return true;
    }
}
