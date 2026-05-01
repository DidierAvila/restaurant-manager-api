using MediatR;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

/// <summary>
/// Command para crear un nuevo usuario
/// </summary>
public record CreateUserCommand(CreateUserDto UserDto) : IRequest<Result<UserDto>>;
