using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers;

public interface IUserMeQueryHandler
{
    Task<UserMeResponseDto> GetUserMe(Guid userId, CancellationToken cancellationToken = default);
}
