using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Permissions;

public class DeletePermission
{
    private readonly IRepositoryBase<Permission> _permissionRepository;

    public DeletePermission(IRepositoryBase<Permission> permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        // Find existing permission
        var permission = await _permissionRepository.Find(x => x.Id == id, cancellationToken);
        if (permission == null)
            throw new KeyNotFoundException("Permission not found");

        // Check if permission has associated roles
        if (permission.Roles?.Any() == true)
            throw new InvalidOperationException("Cannot delete Permission with associated roles. Please remove the permission from all roles first.");

        // Delete permission
        await _permissionRepository.Delete(permission, cancellationToken);
        return true;
    }
}
