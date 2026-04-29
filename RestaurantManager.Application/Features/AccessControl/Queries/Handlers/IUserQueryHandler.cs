using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IUserQueryHandler
{
    Task<UserDto?> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<UserDto>> GetAllUsers(CancellationToken cancellationToken);
    Task<IEnumerable<UserBasicDto>> GetAllUsersBasic(CancellationToken cancellationToken);
    Task<PaginationResponseDto<UserBasicDto>> GetAllUsersFiltered(UserFilterDto filter, CancellationToken cancellationToken);
}
