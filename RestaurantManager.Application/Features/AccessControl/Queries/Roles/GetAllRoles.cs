using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Roles
{
    public class GetAllRoles
    {
        private readonly IRepositoryBase<Role> _roleRepository;
        private readonly IMapper _mapper;

        public GetAllRoles(IRepositoryBase<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDto>> HandleAsync(CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAll(cancellationToken);

            // Map Entities to DTOs using AutoMapper
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }
    }
}
