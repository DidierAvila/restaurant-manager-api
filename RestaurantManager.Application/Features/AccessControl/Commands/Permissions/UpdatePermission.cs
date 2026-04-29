using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Permissions
{
    public class UpdatePermission
    {
        private readonly IRepositoryBase<Permission> _permissionRepository;
        private readonly IMapper _mapper;

        public UpdatePermission(IRepositoryBase<Permission> permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<PermissionDto> HandleAsync(Guid id, UpdatePermissionDto updatePermissionDto, CancellationToken cancellationToken)
        {
            // Find existing permission
            var permission = await _permissionRepository.Find(x => x.Id == id, cancellationToken);
            if (permission == null)
                throw new KeyNotFoundException("Permission not found");

            // Validate that the name doesn't already exist (if it's being updated)
            if (!string.IsNullOrWhiteSpace(updatePermissionDto.Name) && 
                updatePermissionDto.Name != permission.Name)
            {
                var existingPermission = await _permissionRepository.Find(x => x.Name == updatePermissionDto.Name, cancellationToken);
                if (existingPermission != null)
                    throw new InvalidOperationException("A Permission with this name already exists");
            }

            // Map updated values from DTO to existing entity
            _mapper.Map(updatePermissionDto, permission);

            // Update the Permission
            await _permissionRepository.Update(permission, cancellationToken);

            // Map Entity back to DTO for return
            return _mapper.Map<PermissionDto>(permission);
        }
    }
}
