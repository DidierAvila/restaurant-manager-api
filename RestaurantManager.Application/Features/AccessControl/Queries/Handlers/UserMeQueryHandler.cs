using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.UserMe;

namespace RestaurantManager.Application.Core.Auth.Queries.Handlers
{
    public class UserMeQueryHandler : IUserMeQueryHandler
    {
        private readonly GetUserMe _getUserMeQuery;

        public UserMeQueryHandler(GetUserMe getUserMeQuery)
        {
            _getUserMeQuery = getUserMeQuery;
        }

        public async Task<UserMeResponseDto> GetUserMe(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _getUserMeQuery.ExecuteAsync(userId, cancellationToken);
        }
    }
}
