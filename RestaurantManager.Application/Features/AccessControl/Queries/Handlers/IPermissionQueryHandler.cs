using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IPermissionQueryHandler
{
    Task<Result<PermissionDto>> GetPermissionById(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<PermissionDto>>> GetAllPermissions(CancellationToken cancellationToken);
    Task<Result<IEnumerable<PermissionDto>>> GetActivePermissions(CancellationToken cancellationToken);
    Task<Result<IEnumerable<PermissionSummaryDto>>> GetPermissionsSummary(CancellationToken cancellationToken);
    Task<Result<PaginationResponseDto<PermissionListResponseDto>>> GetAllPermissionsFiltered(PermissionFilterDto filter, CancellationToken cancellationToken);
    Task<Result<IEnumerable<PermissionDropdownDto>>> GetPermissionsForDropdown(CancellationToken cancellationToken);
}
