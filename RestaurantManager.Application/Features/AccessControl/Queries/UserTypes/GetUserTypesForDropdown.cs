using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public class GetUserTypesForDropdown
{
    private readonly IRepositoryBase<UserType> _userTypeRepository;

    public GetUserTypesForDropdown(IRepositoryBase<UserType> userTypeRepository)
    {
        _userTypeRepository = userTypeRepository;
    }

    public async Task<IEnumerable<UserTypeDropdownDto>> HandleAsync(CancellationToken cancellationToken)
    {
        // Obtener solo los tipos de usuario activos, ordenados alfabéticamente
        var activeUserTypes = await _userTypeRepository.Finds(x => x.Status, cancellationToken);
        
        // Validar que no sea null y mapear solo Id y Name para máximo rendimiento
        if (activeUserTypes == null)
            return new List<UserTypeDropdownDto>();
        
        return activeUserTypes
            .OrderBy(ut => ut.Name)
            .Select(ut => new UserTypeDropdownDto 
            { 
                Id = ut.Id, 
                Name = ut.Name 
            })
            .ToList();
    }
}
