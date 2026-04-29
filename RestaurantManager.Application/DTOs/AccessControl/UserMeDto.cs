namespace RestaurantManager.Application.DTOs.AccessControl;

/// <summary>
/// DTO que contiene toda la información de contexto del usuario autenticado
/// Incluye perfil, permisos, roles, configuraciones y datos necesarios para el frontend
/// </summary>
public class UserMeDto
{
    // Información del Usuario
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    // Información del Tipo de Usuario
    public Guid UserTypeId { get; set; }
    public string? UserTypeName { get; set; }
    
    // Roles del Usuario
    public List<UserRoleDto> Roles { get; set; } = [];
    
    // Permisos del Usuario (agregados de todos los roles) - Optimizado
    public List<PermissionBasicDto> Permissions { get; set; } = [];

    // Permisos agrupados por recurso para frontend (formato híbrido)
    public Dictionary<string, Dictionary<string, bool>> PermissionsByResource { get; set; } = [];

    // Configuraciones del Usuario
    public UserConfigurationDto Configuration { get; set; } = new();
    
    // Menús/Navegación disponibles según permisos
    public List<MenuItemDto> AvailableMenus { get; set; } = [];

    // Datos adicionales del usuario
    public Dictionary<string, object>? AdditionalData { get; set; }
    
    // Información de la sesión
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool Status { get; set; }
}

public class UserConfigurationDto
{
    public string Language { get; set; } = "es-ES";
    public string Timezone { get; set; } = "America/Mexico_City";
    public string Theme { get; set; } = "light"; // light, dark
    public string DateFormat { get; set; } = "dd/MM/yyyy";
    public string TimeFormat { get; set; } = "HH:mm";
    public bool NotificationsEnabled { get; set; } = true;
    public Dictionary<string, object>? CustomSettings { get; set; }
}

public class MenuItemDto
{
    public string Id { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int Order { get; set; }
    public bool IsGroup { get; set; }
    public List<MenuItemDto>? Children { get; set; }
    public List<string>? RequiredPermissions { get; set; }
}
