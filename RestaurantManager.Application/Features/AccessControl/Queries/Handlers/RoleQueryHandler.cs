using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Roles;

namespace RestaurantManager.Application.Core.Auth.Queries.Handlers
{
    public class RoleQueryHandler : IRoleQueryHandler
    {
        private readonly GetRoleById _getRoleById;
        private readonly GetAllRoles _getAllRoles;
        private readonly GetAllRolesFiltered _getAllRolesFiltered;
        private readonly GetRolesDropdown _getRolesDropdown;

        public RoleQueryHandler(
            GetRoleById getRoleById, 
            GetAllRoles getAllRoles,
            GetAllRolesFiltered getAllRolesFiltered,
            GetRolesDropdown getRolesDropdown)
        {
            _getRoleById = getRoleById;
            _getAllRoles = getAllRoles;
            _getAllRolesFiltered = getAllRolesFiltered;
            _getRolesDropdown = getRolesDropdown;
        }

        public async Task<RoleDto?> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            return await _getRoleById.HandleAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<RoleDto>> GetAllRoles(CancellationToken cancellationToken)
        {
            return await _getAllRoles.HandleAsync(cancellationToken);
        }

        public async Task<PaginationResponseDto<RoleListResponseDto>> GetAllRolesFiltered(RoleFilterDto filter, CancellationToken cancellationToken)
        {
            return await _getAllRolesFiltered.GetRolesFiltered(filter, cancellationToken);
        }

        public async Task<IEnumerable<RoleDropdownDto>> GetRolesDropdown(CancellationToken cancellationToken)
        {
            return await _getRolesDropdown.HandleAsync(cancellationToken);
        }
    }
}
