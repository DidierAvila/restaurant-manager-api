using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Queries.Users;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Handlers
{
    public class UserQueryHandler : IUserQueryHandler
    {
        private readonly GetUserById _getUserById;
        private readonly GetAllUsers _getAllUsers;
        private readonly GetAllUsersBasic _getAllUsersBasic;
        private readonly GetAllUsersFiltered _getAllUsersFiltered;

        public UserQueryHandler(
            GetUserById getUserById, 
            GetAllUsers getAllUsers,
            GetAllUsersBasic getAllUsersBasic,
            GetAllUsersFiltered getAllUsersFiltered)
        {
            _getUserById = getUserById;
            _getAllUsers = getAllUsers;
            _getAllUsersBasic = getAllUsersBasic;
            _getAllUsersFiltered = getAllUsersFiltered;
        }

        public async Task<UserDto?> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            return await _getUserById.HandleAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers(CancellationToken cancellationToken)
        {
            return await _getAllUsers.HandleAsync(cancellationToken);
        }

        public async Task<IEnumerable<UserBasicDto>> GetAllUsersBasic(CancellationToken cancellationToken)
        {
            return await _getAllUsersBasic.HandleAsync(cancellationToken);
        }

        public async Task<PaginationResponseDto<UserBasicDto>> GetAllUsersFiltered(UserFilterDto filter, CancellationToken cancellationToken)
        {
            return await _getAllUsersFiltered.HandleAsync(filter, cancellationToken);
        }
    }
}
