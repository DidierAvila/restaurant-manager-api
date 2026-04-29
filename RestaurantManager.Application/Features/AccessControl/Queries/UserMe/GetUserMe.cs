using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;
using System.Text.Json;

namespace RestaurantManager.Application.Features.AccessControl.Queries.UserMe;

public class GetUserMe
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMapper _mapper;

    public GetUserMe(
        IRepositoryBase<User> userRepository,
        IRepositoryBase<Role> roleRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMenuRepository menuRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _menuRepository = menuRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene la información del usuario autenticado en formato híbrido con navegación y permisos agrupados
    /// Formato optimizado para frontends con navegación dinámica y permisos granulares
    /// </summary>
    public async Task<UserMeResponseDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
        {
            return new UserMeResponseDto
            {
                Success = false,
                Data = {},
            };
        }

        var user = await _userRepository.Find(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new UserMeResponseDto
            {
                Success = false,
                Data = { },
            };
        }

        // Obtener roles del usuario (solo activos)
        var userRoles = await GetUserRolesOptimizedAsync(userId, cancellationToken);

        UserTypeDto? portalConfigDto = null;

        // Crear respuesta en formato híbrido
        return new UserMeResponseDto
        {
            Success = true,
            Data = new UserMeDataDto
            {
                User = new UserBasicInfoDto
                {
                    Id = user.Id.ToString(),
                    Name = user.Name,
                    Email = user.Email,
                    Avatar = user.Image ?? "/images/users/default.jpg"
                },
                Roles = userRoles,
                PortalConfiguration = portalConfigDto
            }
        };
    }



    /// <summary>
    /// Método optimizado para obtener roles del usuario usando UserRoleRepository
    /// </summary>
    private async Task<List<UserRoleDto>> GetUserRolesOptimizedAsync(Guid userId, CancellationToken cancellationToken)
    {
        var roles = new List<UserRoleDto>();

        // Obtener relaciones UserRole del usuario
        var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, cancellationToken);
        
        // Consultar detalles de cada rol
        foreach (var userRole in userRoles)
        {
            var role = await _roleRepository.Find(r => r.Id == userRole.RoleId, cancellationToken);
            if (role != null && role.Status)
            {
                roles.Add(new UserRoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = !string.IsNullOrWhiteSpace(role.Description) ? role.Description : null,
                    Status = role.Status
                });
            }
        }
        
        return roles.OrderBy(r => r.Name).ToList();
    }
    
    /// <summary>
    /// Parsea la configuración adicional desde un string JSON
    /// </summary>
    private static JsonElement? ParseAdditionalConfig(string? additionalConfig)
    {
        if (string.IsNullOrEmpty(additionalConfig))
            return null;

        try
        {
            using var document = JsonDocument.Parse(additionalConfig);
            return document.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Obtiene los IDs de permisos del usuario basados en sus roles
    /// </summary>
    private async Task<List<Guid>> GetUserPermissionIdsAsync(List<UserRoleDto> userRoles, CancellationToken cancellationToken)
    {
        var permissionIds = new HashSet<Guid>();
        
        foreach (var role in userRoles)
        {
            var rolePermissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(role.Id, cancellationToken);
            foreach (var rolePermission in rolePermissions)
            {
                permissionIds.Add(rolePermission.PermissionId);
            }
        }
        
        return permissionIds.ToList();
    }

    /// <summary>
    /// Obtiene los menús disponibles basados en los permisos del usuario
    /// </summary>
    private async Task<List<Menu>> GetMenusForPermissionsAsync(List<Guid> permissionIds, CancellationToken cancellationToken)
    {
        var menuIds = new HashSet<Guid>();
        
        // Obtener menús asociados a los permisos del usuario
        //foreach (var permissionId in permissionIds)
        //{
        //    var menuPermissions = await _menuPermissionRepository.GetMenusByPermissionIdAsync(permissionId, cancellationToken);
        //    foreach (var menuPermission in menuPermissions)
        //    {
        //        menuIds.Add(menuPermission.MenuId);
        //    }
        //}
        
        // Obtener los detalles de los menús y filtrar solo los activos
        var menus = new List<Menu>();
        foreach (var menuId in menuIds)
        {
            var menu = await _menuRepository.Find(m => m.Id == menuId && m.Status, cancellationToken);
            if (menu != null)
            {
                menus.Add(menu);
            }
        }
        
        return menus.OrderBy(m => m.Order).ToList();
    }

    /// <summary>
    /// Construye la estructura de navegación jerárquica desde los menús
    /// </summary>
    private List<NavigationItemDto> BuildNavigationFromMenus(List<Menu> menus)
    {
        var navigation = new List<NavigationItemDto>();
        
        // Obtener menús raíz (sin padre)
        var rootMenus = menus.Where(m => m.ParentId == null || m.ParentId == Guid.Empty).OrderBy(m => m.Order);
        
        foreach (var rootMenu in rootMenus)
        {
            var navItem = new NavigationItemDto
            {
                Id = rootMenu.Id.ToString(),
                Label = rootMenu.Label,
                Icon = rootMenu.Icon,
                Route = rootMenu.Route,
                Children = BuildNavigationChildren(rootMenu.Id, menus)
            };
            
            navigation.Add(navItem);
        }
        
        return navigation;
    }

    /// <summary>
    /// Construye recursivamente los elementos hijos de navegación
    /// </summary>
    private List<NavigationItemDto> BuildNavigationChildren(Guid parentId, List<Menu> allMenus)
    {
        var children = new List<NavigationItemDto>();
        
        var childMenus = allMenus.Where(m => m.ParentId == parentId).OrderBy(m => m.Order);
        
        foreach (var childMenu in childMenus)
        {
            var navItem = new NavigationItemDto
            {
                Id = childMenu.Id.ToString(),
                Label = childMenu.Label,
                Icon = childMenu.Icon,
                Route = childMenu.Route,
                Children = BuildNavigationChildren(childMenu.Id, allMenus)
            };
            
            children.Add(navItem);
        }
        
        return children;
    }
}
