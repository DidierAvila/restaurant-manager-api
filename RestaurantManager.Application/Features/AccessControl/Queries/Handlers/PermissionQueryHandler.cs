using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Permissions;

namespace RestaurantManager.Application.Core.Auth.Queries.Handlers
{
    public class PermissionQueryHandler : IPermissionQueryHandler
    {
        private readonly GetPermissionById _getPermissionById;
        private readonly GetAllPermissions _getAllPermissions;
        private readonly GetActivePermissions _getActivePermissions;
        private readonly GetPermissionsSummary _getPermissionsSummary;
        private readonly GetAllPermissionsFiltered _getAllPermissionsFiltered;
        private readonly GetPermissionsForDropdown _getPermissionsForDropdown;

        public PermissionQueryHandler(
            GetPermissionById getPermissionById,
            GetAllPermissions getAllPermissions,
            GetActivePermissions getActivePermissions,
            GetPermissionsSummary getPermissionsSummary,
            GetAllPermissionsFiltered getAllPermissionsFiltered,
            GetPermissionsForDropdown getPermissionsForDropdown)
        {
            _getPermissionById = getPermissionById;
            _getAllPermissions = getAllPermissions;
            _getActivePermissions = getActivePermissions;
            _getPermissionsSummary = getPermissionsSummary;
            _getAllPermissionsFiltered = getAllPermissionsFiltered;
            _getPermissionsForDropdown = getPermissionsForDropdown;
        }

        public async Task<PermissionDto?> GetPermissionById(Guid id, CancellationToken cancellationToken)
        {
            return await _getPermissionById.HandleAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<PermissionDto>> GetAllPermissions(CancellationToken cancellationToken)
        {
            return await _getAllPermissions.HandleAsync(cancellationToken);
        }

        public async Task<IEnumerable<PermissionDto>> GetActivePermissions(CancellationToken cancellationToken)
        {
            return await _getActivePermissions.HandleAsync(cancellationToken);
        }

        public async Task<IEnumerable<PermissionSummaryDto>> GetPermissionsSummary(CancellationToken cancellationToken)
        {
            return await _getPermissionsSummary.HandleAsync(cancellationToken);
        }

        public async Task<PaginationResponseDto<PermissionListResponseDto>> GetAllPermissionsFiltered(PermissionFilterDto filter, CancellationToken cancellationToken)
        {
            return await _getAllPermissionsFiltered.GetPermissionsFiltered(filter, cancellationToken);
        }

        public async Task<IEnumerable<PermissionDropdownDto>> GetPermissionsForDropdown(CancellationToken cancellationToken)
        {
            return await _getPermissionsForDropdown.HandleAsync(cancellationToken);
        }
    }
}
