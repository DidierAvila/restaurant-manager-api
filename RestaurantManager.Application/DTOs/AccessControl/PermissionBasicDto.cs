namespace RestaurantManager.Application.DTOs.AccessControl;

/// <summary>
/// DTO básico para permisos (optimizado para rendimiento)
/// </summary>
public class PermissionBasicDto
{
    /// <summary>
    /// ID del permiso
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nombre del permiso
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
