
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Domain.Repositories.AccessControl;

public interface IRolePermissionRepository : IRepositoryBase<RolePermission>
{
    Task<IEnumerable<RolePermission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RolePermission>> GetRolesByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default);
    Task<RolePermission?> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RolePermission>> GetRolePermissionsWithDetailsAsync(CancellationToken cancellationToken = default);
    Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    Task RemoveAllPermissionsFromRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<RolePermission?> GetByCompositeIdAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
}
