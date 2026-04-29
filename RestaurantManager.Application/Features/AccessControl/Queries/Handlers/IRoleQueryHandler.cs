using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IRoleQueryHandler
{
    Task<RoleDto?> GetRoleById(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<RoleDto>> GetAllRoles(CancellationToken cancellationToken);
    Task<PaginationResponseDto<RoleListResponseDto>> GetAllRolesFiltered(RoleFilterDto filter, CancellationToken cancellationToken);
    Task<IEnumerable<RoleDropdownDto>> GetRolesDropdown(CancellationToken cancellationToken);
}
