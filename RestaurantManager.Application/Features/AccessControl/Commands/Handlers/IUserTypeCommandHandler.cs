using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Handlers;

public interface IUserTypeCommandHandler
{
    Task<UserTypeDto> CreateUserType(CreateUserTypeDto command, CancellationToken cancellationToken);
    Task<UserTypeDto> UpdateUserType(Guid id, UpdateUserTypeDto command, CancellationToken cancellationToken);
    Task<bool> DeleteUserType(Guid id, CancellationToken cancellationToken);
}
