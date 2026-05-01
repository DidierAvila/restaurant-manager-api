using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;

public class AssignPermissionToRole
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IMapper _mapper;

    public AssignPermissionToRole(
        IRolePermissionRepository rolePermissionRepository,
        IRepositoryBase<Role> roleRepository,
        IRepositoryBase<Permission> permissionRepository,
        IMapper mapper)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<Result<RolePermissionDto>> HandleAsync(AssignPermissionToRoleDto request, CancellationToken cancellationToken = default)
    {
        // Verificar que el rol existe
        var role = await _roleRepository.GetByID(request.RoleId, cancellationToken);
        if (role == null)
        {
            return Result.Failure<RolePermissionDto>(Error.NotFound("Role.NotFound", $"Rol con ID {request.RoleId} no encontrado"));
        }

        // Verificar que el permiso existe
        var permission = await _permissionRepository.GetByID(request.PermissionId, cancellationToken);
        if (permission == null)
        {
            return Result.Failure<RolePermissionDto>(Error.NotFound("Permission.NotFound", $"Permiso con ID {request.PermissionId} no encontrado"));
        }

        // Verificar si ya existe la asignación
        var exists = await _rolePermissionRepository.ExistsAsync(request.RoleId, request.PermissionId, cancellationToken);
        if (exists)
        {
            return Result.Failure<RolePermissionDto>(Error.Conflict("RolePermission.AlreadyExists", $"El permiso '{permission.Name}' ya está asignado al rol '{role.Name}'"));
        }

        var entity = _mapper.Map<RolePermission>(request);

        var createdEntity = await _rolePermissionRepository.Create(entity, cancellationToken);
        return Result.Success(_mapper.Map<RolePermissionDto>(createdEntity));
    }
}
