using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IRoleQueryHandler
{
    Task<Result<RoleDto>> GetRoleById(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken cancellationToken);
    Task<Result<PaginationResponseDto<RoleListResponseDto>>> GetAllRolesFiltered(RoleFilterDto filter, CancellationToken cancellationToken);
    Task<Result<IEnumerable<RoleDropdownDto>>> GetRolesDropdown(CancellationToken cancellationToken);
}
