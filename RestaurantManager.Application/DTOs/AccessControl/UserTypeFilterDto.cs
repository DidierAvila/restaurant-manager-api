
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class UserTypeFilterDto : PaginationRequestDto
{
    /// <summary>
    /// Búsqueda general en nombre y descripción del tipo de usuario
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Filtrar por nombre específico del tipo de usuario
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Filtrar por estado del tipo de usuario (activo/inactivo)
    /// </summary>
    public bool? Status { get; set; }
}

public class UserTypeListResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public int UserCount { get; set; }
    public string? Theme { get; set; }
    public string? DefaultLandingPage { get; set; }
    public string? LogoUrl { get; set; }
    public string? Language { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
