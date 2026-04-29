using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Permissions;

public class GetPermissionsSummary
{
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IMapper _mapper;

    public GetPermissionsSummary(IRepositoryBase<Permission> permissionRepository, IMapper mapper)
    {
        _permissionRepository = permissionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PermissionSummaryDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var permissions = await _permissionRepository.GetAll(cancellationToken);
        return _mapper.Map<IEnumerable<PermissionSummaryDto>>(permissions);
    }
}
