using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories.AccessControl;
using RestaurantManager.Infrastructure.DbContexts;

namespace RestaurantManager.Infrastructure.Repositories.Auth
{
    public class MenuRepository : RepositoryBase<Menu>, IMenuRepository
    {
        public MenuRepository(RestaurantManagerDbContext context, ILogger<MenuRepository> logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Menu>> GetMenusByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Menu>> GetActiveMenusAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .Where(m => m.Status)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Menu>> GetMenusOrderedAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<Menu?> GetByRouteAsync(string route, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .FirstOrDefaultAsync(m => m.Route == route, cancellationToken);
        }

        public async Task<IEnumerable<Menu>> GetGroupMenusAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .Where(m => m.IsGroup == true)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Menu>> GetChildMenusAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .Where(m => m.ParentId == parentId && m.IsGroup == false)
                .OrderBy(m => m.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasChildMenusAsync(Guid menuId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Menu>()
                .AnyAsync(m => m.ParentId == menuId, cancellationToken);
        }
    }
}
