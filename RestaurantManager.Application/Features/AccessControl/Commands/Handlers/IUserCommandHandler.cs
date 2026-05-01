using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Users;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IUserCommandHandler
{
    Task<Result<UserDto>> CreateUser(CreateUserDto command, CancellationToken cancellationToken);
    Task<Result<UserDto>> UpdateUser(Guid id, UpdateUserDto command, CancellationToken cancellationToken);
    Task<Result> DeleteUser(Guid id, CancellationToken cancellationToken);
    Task<Result> ChangePassword(Guid userId, ChangePasswordDto command, CancellationToken cancellationToken);

    // Role management methods
    Task<Result<MultipleRoleAssignmentResult>> AssignMultipleRolesToUser(AssignMultipleRolesToUser command, CancellationToken cancellationToken);
    Task<Result<MultipleRoleRemovalResult>> RemoveMultipleRolesFromUser(RemoveMultipleRolesFromUser command, CancellationToken cancellationToken);
    Task<Result<List<UserRoleDto>>> GetUserRoles(Guid userId, CancellationToken cancellationToken);

    // Additional Data methods
    Task<Result<UserAdditionalValueResponseDto>> SetUserAdditionalValue(Guid userId, UserAdditionalDataOperationDto operationDto, CancellationToken cancellationToken);
    Task<Result<UserAdditionalValueResponseDto>> GetUserAdditionalValue(Guid userId, string key, CancellationToken cancellationToken);
    Task<Result> RemoveUserAdditionalValue(Guid userId, string key, CancellationToken cancellationToken);
    Task<Result<Dictionary<string, object>>> UpdateUserAdditionalData(Guid userId, UpdateUserAdditionalDataDto updateDto, CancellationToken cancellationToken);
}
