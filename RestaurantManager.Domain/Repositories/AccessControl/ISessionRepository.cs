
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Domain.Repositories.AccessControl;

/// <summary>
/// Repositorio específico para manejar sesiones
/// </summary>
public interface ISessionRepository : IRepositoryBase<Session>
{
    /// <summary>
    /// Eliminar todas las sesiones de usuarios que tienen un rol específico
    /// </summary>
    Task<int> DeleteSessionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eliminar todas las sesiones de usuarios que tienen cualquiera de los roles especificados
    /// </summary>
    Task<int> DeleteSessionsByRoleIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eliminar todas las sesiones de un usuario específico
    /// </summary>
    Task<int> DeleteSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener todas las sesiones activas (no expiradas)
    /// </summary>
    Task<List<Session>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener sesiones de usuarios con un rol específico
    /// </summary>
    Task<List<Session>> GetSessionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
}
