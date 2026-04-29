using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Users;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IUserCommandHandler
{
    Task<UserDto> CreateUser(CreateUserDto command, CancellationToken cancellationToken);
    Task<UserDto> UpdateUser(Guid id, UpdateUserDto command, CancellationToken cancellationToken);
    Task<bool> DeleteUser(Guid id, CancellationToken cancellationToken);
    Task<bool> ChangePassword(Guid userId, ChangePasswordDto command, CancellationToken cancellationToken);
    
    // Role management methods
    Task<MultipleRoleRemovalResult> RemoveMultipleRolesFromUser(RemoveMultipleRolesFromUser command, CancellationToken cancellationToken);
    Task<List<UserRoleDto>> GetUserRoles(Guid userId, CancellationToken cancellationToken);
    
    // Additional Data methods
    Task<UserAdditionalValueResponseDto> SetUserAdditionalValue(Guid userId, UserAdditionalDataOperationDto operationDto, CancellationToken cancellationToken);
    Task<UserAdditionalValueResponseDto> GetUserAdditionalValue(Guid userId, string key, CancellationToken cancellationToken);
    Task<bool> RemoveUserAdditionalValue(Guid userId, string key, CancellationToken cancellationToken);
    Task<Dictionary<string, object>> UpdateUserAdditionalData(Guid userId, UpdateUserAdditionalDataDto updateDto, CancellationToken cancellationToken);
}
