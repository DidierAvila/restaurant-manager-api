
using RestaurantManager.Application.DTOs.Common;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class UserFilterDto : PaginationRequestDto
{
    /// <summary>
    /// Búsqueda general en nombre, email y teléfono del usuario
    /// </summary>
    public string? Search { get; set; }
    
    /// <summary>
    /// Filtrar por nombre específico del usuario
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Filtrar por email específico del usuario
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Filtrar por ID del rol asignado al usuario
    /// </summary>
    public Guid? RoleId { get; set; }
    
    /// <summary>
    /// Filtrar por ID del tipo de usuario
    /// </summary>
    public Guid? UserTypeId { get; set; }
    
    /// <summary>
    /// Filtrar usuarios creados después de esta fecha
    /// </summary>
    public DateTime? CreatedAfter { get; set; }
    
    /// <summary>
    /// Filtrar usuarios creados antes de esta fecha
    /// </summary>
    public DateTime? CreatedBefore { get; set; }
}
