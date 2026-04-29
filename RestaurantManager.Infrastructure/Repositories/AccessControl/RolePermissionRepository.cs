using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories.AccessControl;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Infrastructure.Repositories.Auth
{
    public class RolePermissionRepository : RepositoryBase<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(RestaurantManagerDbContext context, ILogger<RolePermissionRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<RolePermission>> GetPermissionsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Permission)
                .Include(rp => rp.Role)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<RolePermission>> GetRolesByPermissionIdAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.PermissionId == permissionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<RolePermission?> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
        }

        public async Task<IEnumerable<RolePermission>> GetRolePermissionsWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .OrderBy(rp => rp.Role.Name)
                .ThenBy(rp => rp.Permission.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task RemovePermissionFromRoleAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            var rolePermission = await GetByRoleAndPermissionAsync(roleId, permissionId, cancellationToken);
            if (rolePermission != null)
            {
                _context.Set<RolePermission>().Remove(rolePermission);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RemoveAllPermissionsFromRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            var rolePermissions = await _context.Set<RolePermission>()
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync(cancellationToken);

            if (rolePermissions.Any())
            {
                _context.Set<RolePermission>().RemoveRange(rolePermissions);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public new async Task<IEnumerable<RolePermission>> GetAll(CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .ToListAsync(cancellationToken);
        }

        public async Task<RolePermission?> GetByCompositeIdAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<RolePermission>()
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
        }

        // Hide base GetByID to handle composite key case
        public new Task<RolePermission?> GetByID(Guid id, CancellationToken cancellationToken = default)
        {
            // For composite key entities, this method doesn't make sense
            // Return null or throw exception based on your preference
            throw new NotSupportedException("RolePermission uses composite keys. Use GetByCompositeIdAsync or GetByRoleAndPermissionAsync instead.");
        }
    }
}
