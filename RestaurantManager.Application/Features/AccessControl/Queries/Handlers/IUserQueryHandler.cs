using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IUserQueryHandler
{
    Task<Result<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken);
    Task<Result<IEnumerable<UserBasicDto>>> GetAllUsersBasic(CancellationToken cancellationToken);
    Task<Result<PaginationResponseDto<UserBasicDto>>> GetAllUsersFiltered(UserFilterDto filter, CancellationToken cancellationToken);
}
