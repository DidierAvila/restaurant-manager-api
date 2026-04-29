using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;

public class CreateRolePermission
{
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMapper _mapper;

    public CreateRolePermission(IRolePermissionRepository rolePermissionRepository, IMapper mapper)
    {
        _rolePermissionRepository = rolePermissionRepository;
        _mapper = mapper;
    }

    public async Task<RolePermissionDto> HandleAsync(CreateRolePermissionDto request, CancellationToken cancellationToken = default)
    {
        // Verificar si ya existe la asignación
        var exists = await _rolePermissionRepository.ExistsAsync(request.RoleId, request.PermissionId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"El permiso ya está asignado a este rol");
        }

        var entity = _mapper.Map<RolePermission>(request);
        
        var createdEntity = await _rolePermissionRepository.Create(entity, cancellationToken);
        return _mapper.Map<RolePermissionDto>(createdEntity);
    }
}
