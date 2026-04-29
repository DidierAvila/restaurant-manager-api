using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Roles;

public class GetRoleById
{
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMapper _mapper;

    public GetRoleById(
        IRepositoryBase<Role> roleRepository, 
        IRepositoryBase<Permission> permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMapper mapper)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
    }

    public async Task<RoleDto?> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.Find(x => x.Id == id, cancellationToken);
        if (role == null)
            return null;

        // Obtener los permisos asociados al rol a través de la tabla intermedia RolePermission
        var rolePermissionRelations = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(id, cancellationToken);
        var permissionIds = rolePermissionRelations.Select(rp => rp.PermissionId).ToList();

        // Obtener los permisos completos
        var permissions = await _permissionRepository.Finds(p => permissionIds.Contains(p.Id), cancellationToken);

        // Verificar si permissions no es null antes de continuar
        if (permissions != null && permissions.Any())
        {
            // Usar LINQ para simplificar el bucle y evitar la desreferencia de null
            var permissionDropdownDtos = permissions
                .Where(permission => permission != null)
                .Select(permission => new PermissionDropdownDto { Id = permission.Id, Name = permission.Name })
                .ToList();

            // Map Entity to DTO using AutoMapper
            var roleDto = _mapper.Map<RoleDto>(role);

            // Asignar los permisos manualmente
            roleDto.Permissions = permissionDropdownDtos;

            return roleDto;
        }

        // Map Entity to DTO using AutoMapper
        var roleDtoWithoutPermissions = _mapper.Map<RoleDto>(role);
        roleDtoWithoutPermissions.Permissions = [];

        return roleDtoWithoutPermissions;
    }
}
