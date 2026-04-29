
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class RoleFilterDto : PaginationRequestDto
{
    /// <summary>
    /// Búsqueda general en nombre y descripción del rol
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Filtrar por nombre específico del rol
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Filtrar por estado del rol (activo/inactivo)
    /// </summary>
    public bool? Status { get; set; }
}

public class RoleListResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public int UserCount { get; set; } // Cantidad de usuarios con este rol
    public int PermissionCount { get; set; } // Cantidad de permisos asignados
    public DateTime CreatedAt { get; set; }
}
