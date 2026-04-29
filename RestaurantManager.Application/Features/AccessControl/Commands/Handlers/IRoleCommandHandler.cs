using RestaurantManager.Application.Core.Auth.Commands.RolePermissions;
using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IRoleCommandHandler
{
    Task<RoleDto> CreateRole(CreateRoleDto command, CancellationToken cancellationToken);
    Task<RoleDto> UpdateRole(Guid id, UpdateRoleDto command, CancellationToken cancellationToken);
    Task<bool> DeleteRole(Guid id, CancellationToken cancellationToken);
    
    // Permission management methods
    Task<MultiplePermissionRemovalResult> RemoveMultiplePermissionsFromRole(RemoveMultiplePermissionsFromRole command, CancellationToken cancellationToken);
}
