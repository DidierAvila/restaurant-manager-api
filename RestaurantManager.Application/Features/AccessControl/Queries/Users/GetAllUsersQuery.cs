using MediatR;
using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Users;

/// <summary>
/// Query para obtener todos los usuarios
/// </summary>
public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;
