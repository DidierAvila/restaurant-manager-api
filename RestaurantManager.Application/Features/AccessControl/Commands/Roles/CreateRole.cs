using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Queries.Permissions;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Roles
{
    public class CreateRole
    {
        private readonly IRepositoryBase<Role> _roleRepository;
        private readonly IRepositoryBase<Permission> _permissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;
        private readonly GetPermissionsForDropdown _getPermissionsForDropdown;
        private readonly IMapper _mapper;

        public CreateRole(IRepositoryBase<Role> roleRepository, IRepositoryBase<Permission> permissionRepository, IRolePermissionRepository rolePermissionRepository, GetPermissionsForDropdown getPermissionsForDropdown, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _getPermissionsForDropdown = getPermissionsForDropdown;
            _mapper = mapper;
        }

        public async Task<RoleDto> HandleAsync(CreateRoleDto createRoleDto, CancellationToken cancellationToken)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(createRoleDto.Name))
                throw new ArgumentException("Name is required");

            // Check if role already exists
            var existingRole = await _roleRepository.Find(x => x.Name == createRoleDto.Name, cancellationToken);
            if (existingRole != null)
                throw new InvalidOperationException("Role with this name already exists");

            // Map DTO to Entity using AutoMapper
            var role = _mapper.Map<Role>(createRoleDto);

            // Create role in repository
            var createdRole = await _roleRepository.Create(role, cancellationToken);

            // Asignar permisos si se proporcionaron
            if (createRoleDto.PermissionIds != null && createRoleDto.PermissionIds.Any())
            {
                await AssignPermissionsToRole(createdRole.Id, createRoleDto.PermissionIds, cancellationToken);
            }

            // Map Entity to DTO using AutoMapper
            var roleDto = _mapper.Map<RoleDto>(createdRole);

            // Cargar permisos asignados para incluir en la respuesta
            if (createRoleDto.PermissionIds != null && createRoleDto.PermissionIds.Any())
            {
                roleDto.Permissions = (List<PermissionDropdownDto>?)await _getPermissionsForDropdown.HandleAsync(roleDto.Id, cancellationToken);
            }

            return roleDto;
        }

        /// <summary>
        /// Asigna múltiples permisos a un rol recién creado
        /// </summary>
        private async Task AssignPermissionsToRole(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken)
        {
            // Validar que todos los permisos existen
            var validPermissions = new List<Permission>();
            foreach (var permissionId in permissionIds)
            {
                var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                if (permission != null)
                {
                    validPermissions.Add(permission);

                    // Crear la relación RolePermission
                    var rolePermission = new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permissionId
                    };

                    await _rolePermissionRepository.Create(rolePermission, cancellationToken);
                }
            }
        }

        private async Task<List<PermissionDto>> LoadRolePermissions(Guid roleId, CancellationToken cancellationToken)
        {
            var rolePermissionRelations = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(roleId, cancellationToken);
            var permissionIds = rolePermissionRelations.Select(rp => rp.PermissionId).ToList();

            var permissions = new List<Permission>();
            foreach (var permissionId in permissionIds)
            {
                var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                if (permission != null)
                {
                    permissions.Add(permission);
                }
            }

            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}
