using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Queries.RolePermissions;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Permissions;

public class GetPermissionsForDropdown
{
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly GetPermissionsByRole _getPermissionsByRole;
    private readonly IMapper _mapper;

    public GetPermissionsForDropdown(IRepositoryBase<Permission> permissionRepository, GetPermissionsByRole getPermissionsByRole, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _getPermissionsByRole = getPermissionsByRole;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PermissionDropdownDto>> HandleAsync(CancellationToken cancellationToken)
    {
        // Obtener solo los permisos activos, ordenados alfabéticamente
        var activePermissions = await _permissionRepository.Finds(x => x.Status, cancellationToken);
        
        // Validar que no sea null y mapear solo Id y Name para máximo rendimiento
        if (activePermissions == null)
            return [];
        
        return activePermissions
            .OrderBy(p => p.Name)
            .Select(p => new PermissionDropdownDto 
            { 
                Id = p.Id, 
                Name = p.Name 
            })
            .ToList();
    }

    public async Task<IEnumerable<PermissionDropdownDto>> HandleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        // Obtener los permisos asociados a roles activos
        var rolesWithPermissions = await _getPermissionsByRole.HandleAsync(roleId, cancellationToken);
        if (rolesWithPermissions == null || !rolesWithPermissions.Any())
            return new List<PermissionDropdownDto>();

        // Extraer los IDs de permisos de roles activos
        var rolePermissionIds = rolesWithPermissions.Select(rp => rp.PermissionId).ToHashSet();

        // Obtener solo los permisos activos, ordenados alfabéticamente
        var activePermissions = await _permissionRepository.Finds(x => x.Status && rolePermissionIds.Contains(x.Id), cancellationToken);

        // Validar que no sea null y mapear solo Id y Name para máximo rendimiento
        if (activePermissions == null)
            return [];

        return [.. activePermissions
            .OrderBy(p => p.Name)
            .Select(p => new PermissionDropdownDto
            {
                Id = p.Id,
                Name = p.Name
            })];
    }
}
