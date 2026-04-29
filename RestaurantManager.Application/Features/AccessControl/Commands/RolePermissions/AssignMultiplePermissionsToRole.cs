using AutoMapper;
using Microsoft.Extensions.Logging;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;

public class AssignMultiplePermissionsToRole
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AssignMultiplePermissionsToRole> _logger;

    public AssignMultiplePermissionsToRole(
        IRolePermissionRepository rolePermissionRepository,
        IRepositoryBase<Role> roleRepository,
        IRepositoryBase<Permission> permissionRepository,
        IMapper mapper,
        ILogger<AssignMultiplePermissionsToRole> logger)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<MultiplePermissionAssignmentResult> HandleAsync(AssignMultiplePermissionsToRoleDto request, CancellationToken cancellationToken = default)
    {
        var result = new MultiplePermissionAssignmentResult
        {
            RoleId = request.RoleId
        };

        // Verificar que el rol existe
        var role = await _roleRepository.Find(r => r.Id == request.RoleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Role with ID {request.RoleId} not found");
        }

        result.RoleName = role.Name;

        // Obtener los permisos existentes del rol para evitar duplicados
        var existingRolePermissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(request.RoleId, cancellationToken);
        var existingPermissionIds = existingRolePermissions?.Select(rp => rp.PermissionId).ToHashSet() ?? new HashSet<Guid>();

        // Procesar cada permiso individualmente
        foreach (var permissionId in request.PermissionIds)
        {
            try
            {
                // Verificar si ya existe la asignación
                if (existingPermissionIds.Contains(permissionId))
                {
                    var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                    result.ExistingPermissions.Add(permission?.Name ?? permissionId.ToString());
                    continue;
                }

                // Verificar que el permiso existe
                var permissionToAssign = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                if (permissionToAssign == null)
                {
                    result.FailedAssignments.Add(new PermissionAssignmentError
                    {
                        PermissionId = permissionId,
                        PermissionName = permissionId.ToString(),
                        ErrorMessage = $"Permission with ID {permissionId} not found"
                    });
                    continue;
                }

                // Crear la asignación
                var rolePermission = new RolePermission
                {
                    RoleId = request.RoleId,
                    PermissionId = permissionId
                };

                var createdRolePermission = await _rolePermissionRepository.Create(rolePermission, cancellationToken);

                // Agregar al resultado exitoso
                result.SuccessfulAssignments.Add(new RolePermissionDto
                {
                    RoleId = request.RoleId,
                    PermissionId = permissionId,
                    RoleName = role.Name,
                    PermissionName = permissionToAssign.Name
                });
            }
            catch (Exception ex)
            {
                var permission = await _permissionRepository.Find(p => p.Id == permissionId, cancellationToken);
                result.FailedAssignments.Add(new PermissionAssignmentError
                {
                    PermissionId = permissionId,
                    PermissionName = permission?.Name ?? permissionId.ToString(),
                    ErrorMessage = ex.Message
                });
            }
        }

        // Invalidar sesiones solo si se asignaron nuevos permisos exitosamente
        if (result.SuccessfulAssignments.Any())
        {
            try
            {
                _logger.LogInformation("Asignación múltiple de permisos completada para el rol {RoleId}. {SuccessfulCount} permisos asignados. Invalidando sesiones de usuarios con este rol", 
                    request.RoleId, result.SuccessfulAssignments.Count);

                //var invalidatedSessions = await _sessionInvalidationService.InvalidateSessionsByRoleAsync(request.RoleId, cancellationToken);
                
                //_logger.LogInformation("Asignación múltiple de permisos y invalidación de sesiones completada para el rol {RoleId}. {InvalidatedSessions} sesiones invalidadas", 
                //    request.RoleId, invalidatedSessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invalidar sesiones después de asignar permisos al rol {RoleId}", request.RoleId);
                // No lanzamos la excepción para no afectar el resultado de la asignación de permisos
            }
        }
        else
        {
            _logger.LogInformation("No se asignaron nuevos permisos al rol {RoleId}, omitiendo invalidación de sesiones", request.RoleId);
        }

        return result;
    }
}
