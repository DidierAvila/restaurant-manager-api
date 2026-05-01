using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IPermissionCommandHandler
{
    Task<Result<PermissionDto>> CreatePermission(CreatePermissionDto command, CancellationToken cancellationToken);
    Task<Result<PermissionDto>> UpdatePermission(Guid id, UpdatePermissionDto command, CancellationToken cancellationToken);
    Task<Result> DeletePermission(Guid id, CancellationToken cancellationToken);
}
