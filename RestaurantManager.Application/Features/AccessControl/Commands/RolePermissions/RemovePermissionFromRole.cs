using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;

public class RemovePermissionFromRole
{
    private readonly IRolePermissionRepository _rolePermissionRepository;

    public RemovePermissionFromRole(IRolePermissionRepository rolePermissionRepository)
    {
        _rolePermissionRepository = rolePermissionRepository;
    }

    public async Task<Result> HandleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermission = await _rolePermissionRepository.GetByRoleAndPermissionAsync(roleId, permissionId, cancellationToken);
        if (rolePermission == null)
        {
            return Result.Failure(Error.NotFound("RolePermission.NotFound", "No se encontró la asignación del permiso al rol especificado"));
        }

        await _rolePermissionRepository.RemovePermissionFromRoleAsync(roleId, permissionId, cancellationToken);
        return Result.Success();
    }
}
