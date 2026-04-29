using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Domain.Repositories.AccessControl;

public interface IMenuRepository : IRepositoryBase<Menu>
{
    Task<IEnumerable<Menu>> GetMenusByParentIdAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Menu>> GetActiveMenusAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Menu>> GetMenusOrderedAsync(CancellationToken cancellationToken = default);
    Task<Menu?> GetByRouteAsync(string route, CancellationToken cancellationToken = default);
    Task<IEnumerable<Menu>> GetGroupMenusAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Menu>> GetChildMenusAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<bool> HasChildMenusAsync(Guid menuId, CancellationToken cancellationToken = default);
}
