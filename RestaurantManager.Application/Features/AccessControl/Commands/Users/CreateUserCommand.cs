using MediatR;
using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

/// <summary>
/// Command para crear un nuevo usuario
/// </summary>
public record CreateUserCommand(CreateUserDto UserDto) : IRequest<UserDto>;
