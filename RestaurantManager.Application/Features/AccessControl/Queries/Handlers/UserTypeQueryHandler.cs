using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public class UserTypeQueryHandler : IUserTypeQueryHandler
{
    private readonly GetUserTypesForDropdown _getUserTypesForDropdown;

    public UserTypeQueryHandler(
        GetUserTypesForDropdown getUserTypesForDropdown)
    {
        _getUserTypesForDropdown = getUserTypesForDropdown;
    }

    public Task<IEnumerable<UserTypeDto>> GetActiveUserTypes(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserTypeDto>> GetAllUserTypes(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PaginationResponseDto<UserTypeListResponseDto>> GetAllUserTypesFiltered(UserTypeFilterDto filter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<UserTypeDto?> GetUserTypeById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserTypeDropdownDto>> GetUserTypesForDropdown(CancellationToken cancellationToken)
    {
        return await _getUserTypesForDropdown.HandleAsync(cancellationToken);
    }

    public Task<IEnumerable<UserTypeSummaryDto>> GetUserTypesSummary(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
