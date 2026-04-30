using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;
using System.Text.Json;
using Menu = RestaurantManager.Domain.Entities.AccessControl.Menu;

namespace RestaurantManager.Application.Features.AccessControl.Queries.UserMe;

public class GetUserMe
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IRepositoryBase<UserType> _userTypeRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly IMapper _mapper;

    public GetUserMe(
        IRepositoryBase<User> userRepository,
        IRepositoryBase<Role> roleRepository,
        IRepositoryBase<UserType> userTypeRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IMenuRepository menuRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userTypeRepository = userTypeRepository;
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

        // Obtener configuración personalizada del portal desde UserType
        var userType = await _userTypeRepository.GetByID(user.UserTypeId, cancellationToken);
        UserTypeDto? portalConfigDto = null;

        if (userType != null)
        {
            // Crear DTO de configuración a partir de UserType
            portalConfigDto = _mapper.Map<UserTypeDto>(userType);
        }
        else
        {
            // Generar configuración automática desde la BD si no existe
            portalConfigDto = await GenerateDefaultPortalConfigAsync(user.UserTypeId, userRoles, cancellationToken);
        }

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
    /// Genera una configuración de portal por defecto basada en los menús y permisos disponibles en la BD
    /// y la guarda en la base de datos
    /// </summary>
    private async Task<UserTypeDto> GenerateDefaultPortalConfigAsync(Guid userTypeId, List<UserRoleDto> userRoles, CancellationToken cancellationToken)
    {
        // Obtener todos los permisos del usuario basados en sus roles
        var userPermissionIds = await GetUserPermissionIdsAsync(userRoles, cancellationToken);

        // Obtener menús disponibles basados en los permisos del usuario
        var availableMenus = await GetMenusForPermissionsAsync(userPermissionIds, cancellationToken);

        // Construir la navegación jerárquica
        var navigation = BuildNavigationFromMenus(availableMenus);

        // Crear la configuración adicional con los menús
        var additionalConfig = new
        {
            menus = navigation
        };

        // Obtener el UserType existente
        var userType = await _userTypeRepository.GetByID(userTypeId, cancellationToken);

        if (userType != null)
        {
            // Actualizar el UserType con la configuración por defecto
            userType.Theme = "default";
            userType.DefaultLandingPage = "/dashboard";
            userType.Language = "es";
            userType.AdditionalConfig = System.Text.Json.JsonSerializer.Serialize(additionalConfig);
            userType.UpdatedAt = DateTime.Now;

            // Guardar los cambios
            await _userTypeRepository.Update(userType, cancellationToken);

            // Crear DTO de configuración
            return _mapper.Map<UserTypeDto>(userType);
        }

        // Si no existe el UserType, devolver una configuración por defecto sin guardar
        // Crear un UserType por defecto
        var defaultUserType = new UserType
        {
            Id = userTypeId,
            Name = "Default",
            Description = "Default user type",
            Status = true,
            Theme = "default",
            DefaultLandingPage = "/dashboard",
            LogoUrl = "/images/logo.png",
            Language = "es",
            AdditionalConfig = System.Text.Json.JsonSerializer.Serialize(additionalConfig),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        return _mapper.Map<UserTypeDto>(defaultUserType);
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
