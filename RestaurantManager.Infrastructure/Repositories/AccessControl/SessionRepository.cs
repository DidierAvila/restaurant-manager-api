using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories.AccessControl;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Infrastructure.Repositories.Auth
{
    /// <summary>
    /// Implementación del repositorio para manejar sesiones
    /// </summary>
    public class SessionRepository : RepositoryBase<Session>, ISessionRepository
    {
        public SessionRepository(RestaurantManagerDbContext context, ILogger<SessionRepository> logger) : base(context, logger)
        {
        }

        /// <summary>
        /// Eliminar todas las sesiones de usuarios que tienen un rol específico
        /// </summary>
        public async Task<int> DeleteSessionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Obtener los IDs de usuarios que tienen el rol especificado
                var userIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync(cancellationToken);

                if (!userIds.Any())
                    return 0;

                // Eliminar todas las sesiones de esos usuarios
                var sessionsToDelete = await _context.Sessions
                    .Where(s => s.UserId.HasValue && userIds.Contains(s.UserId.Value))
                    .ToListAsync(cancellationToken);

                if (!sessionsToDelete.Any())
                    return 0;

                _context.Sessions.RemoveRange(sessionsToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return sessionsToDelete.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar sesiones por rol {roleId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Eliminar todas las sesiones de usuarios que tienen cualquiera de los roles especificados
        /// </summary>
        public async Task<int> DeleteSessionsByRoleIdsAsync(List<Guid> roleIds, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!roleIds.Any())
                    return 0;

                // Obtener los IDs de usuarios que tienen cualquiera de los roles especificados
                var userIds = await _context.UserRoles
                    .Where(ur => roleIds.Contains(ur.RoleId))
                    .Select(ur => ur.UserId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                if (!userIds.Any())
                    return 0;

                // Eliminar todas las sesiones de esos usuarios
                var sessionsToDelete = await _context.Sessions
                    .Where(s => s.UserId.HasValue && userIds.Contains(s.UserId.Value))
                    .ToListAsync(cancellationToken);

                if (!sessionsToDelete.Any())
                    return 0;

                _context.Sessions.RemoveRange(sessionsToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return sessionsToDelete.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar sesiones por roles: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Eliminar todas las sesiones de un usuario específico
        /// </summary>
        public async Task<int> DeleteSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var sessionsToDelete = await _context.Sessions
                    .Where(s => s.UserId == userId)
                    .ToListAsync(cancellationToken);

                if (!sessionsToDelete.Any())
                    return 0;

                _context.Sessions.RemoveRange(sessionsToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                return sessionsToDelete.Count;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar sesiones del usuario {userId}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtener todas las sesiones activas (no expiradas)
        /// </summary>
        public async Task<List<Session>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Sessions
                    .Where(s => s.Expires > DateTime.Now)
                    .Include(s => s.User)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener sesiones activas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtener sesiones de usuarios con un rol específico
        /// </summary>
        public async Task<List<Session>> GetSessionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Obtener los IDs de usuarios que tienen el rol especificado
                var userIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == roleId)
                    .Select(ur => ur.UserId)
                    .ToListAsync(cancellationToken);

                if (!userIds.Any())
                    return new List<Session>();

                // Obtener las sesiones de esos usuarios
                return await _context.Sessions
                    .Where(s => s.UserId.HasValue && userIds.Contains(s.UserId.Value))
                    .Include(s => s.User)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener sesiones por rol {roleId}: {ex.Message}", ex);
            }
        }
    }
}
