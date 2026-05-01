using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Roles;
using RestaurantManager.Domain.Common;

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

        public async Task<Result<RoleDto>> GetRoleById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _getRoleById.HandleAsync(id, cancellationToken);
                if (role == null)
                {
                    return Result.Failure<RoleDto>(Error.NotFound("Role.NotFound", $"Rol con ID {id} no encontrado"));
                }
                return Result.Success(role);
            }
            catch (Exception ex)
            {
                return Result.Failure<RoleDto>(Error.Failure("Role.GetById", ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoleDto>>> GetAllRoles(CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _getAllRoles.HandleAsync(cancellationToken);
                return Result.Success(roles);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<RoleDto>>(Error.Failure("Role.GetAll", ex.Message));
            }
        }

        public async Task<Result<PaginationResponseDto<RoleListResponseDto>>> GetAllRolesFiltered(RoleFilterDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _getAllRolesFiltered.GetRolesFiltered(filter, cancellationToken);
                return Result.Success(roles);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<PaginationResponseDto<RoleListResponseDto>>(Error.Validation("Role.InvalidFilter", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure<PaginationResponseDto<RoleListResponseDto>>(Error.Failure("Role.GetFiltered", ex.Message));
            }
        }

        public async Task<Result<IEnumerable<RoleDropdownDto>>> GetRolesDropdown(CancellationToken cancellationToken)
        {
            try
            {
                var roles = await _getRolesDropdown.HandleAsync(cancellationToken);
                return Result.Success(roles);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<RoleDropdownDto>>(Error.Failure("Role.GetDropdown", ex.Message));
            }
        }
    }
}
