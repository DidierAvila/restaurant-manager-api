using RestaurantManager.Application.Core.Auth.Commands.RolePermissions;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IRoleCommandHandler
{
    Task<Result<RoleDto>> CreateRole(CreateRoleDto command, CancellationToken cancellationToken);
    Task<Result<RoleDto>> UpdateRole(Guid id, UpdateRoleDto command, CancellationToken cancellationToken);
    Task<Result> DeleteRole(Guid id, CancellationToken cancellationToken);

    // Permission management methods
    Task<Result<MultiplePermissionRemovalResult>> RemoveMultiplePermissionsFromRole(RemoveMultiplePermissionsFromRole command, CancellationToken cancellationToken);
}
