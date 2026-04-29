
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Domain.Repositories.AccessControl;

/// <summary>
/// Repositorio específico para manejar la relación UserRole
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// Asignar un rol a un usuario
    /// </summary>
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remover un rol de un usuario
    /// </summary>
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener todos los roles de un usuario
    /// </summary>
    Task<List<UserRole>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener todos los usuarios de un rol
    /// </summary>
    Task<List<UserRole>> GetRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verificar si un usuario tiene un rol específico
    /// </summary>
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener relaciones UserRole con información de usuarios y roles
    /// </summary>
    Task<List<UserRole>> GetUserRolesWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remover todos los roles de un usuario
    /// </summary>
    Task<bool> RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
