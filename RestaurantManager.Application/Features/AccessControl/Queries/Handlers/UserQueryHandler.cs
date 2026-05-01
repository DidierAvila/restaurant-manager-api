using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Queries.Users;
using RestaurantManager.Domain.Common;

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

        public async Task<Result<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _getUserById.HandleAsync(id, cancellationToken);
                if (user == null)
                {
                    return Result.Failure<UserDto>(Error.NotFound("User.NotFound", $"Usuario con ID {id} no encontrado"));
                }
                return Result.Success(user);
            }
            catch (Exception ex)
            {
                return Result.Failure<UserDto>(Error.Failure("User.GetById", ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _getAllUsers.HandleAsync(cancellationToken);
                return Result.Success(users);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<UserDto>>(Error.Failure("User.GetAll", ex.Message));
            }
        }

        public async Task<Result<IEnumerable<UserBasicDto>>> GetAllUsersBasic(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _getAllUsersBasic.HandleAsync(cancellationToken);
                return Result.Success(users);
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<UserBasicDto>>(Error.Failure("User.GetAllBasic", ex.Message));
            }
        }

        public async Task<Result<PaginationResponseDto<UserBasicDto>>> GetAllUsersFiltered(UserFilterDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _getAllUsersFiltered.HandleAsync(filter, cancellationToken);
                return Result.Success(users);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<PaginationResponseDto<UserBasicDto>>(Error.Validation("User.InvalidFilter", ex.Message));
            }
            catch (Exception ex)
            {
                return Result.Failure<PaginationResponseDto<UserBasicDto>>(Error.Failure("User.GetFiltered", ex.Message));
            }
        }
    }
}
