using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class PermissionFilterDto : PaginationRequestDto
{
    /// <summary>
    /// Búsqueda general en nombre y descripción del permiso
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Filtrar por nombre específico del permiso
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Filtrar por estado del permiso (activo/inactivo)
    /// </summary>
    public bool? Status { get; set; }
}

public class PermissionListResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public int RoleCount { get; set; } // Cantidad de roles que tienen este permiso
    public DateTime CreatedAt { get; set; }
}
