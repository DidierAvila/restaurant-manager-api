namespace RestaurantManager.Application.DTOs.AccessControl;

/// <summary>
/// DTO para elementos de navegación con soporte para estructura jerárquica
/// </summary>
public class NavigationItemDto
{
    public string Id { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Route { get; set; } = null!;
    public List<NavigationItemDto> Children { get; set; } = new();
}
