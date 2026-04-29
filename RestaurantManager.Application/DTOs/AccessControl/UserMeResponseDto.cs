namespace RestaurantManager.Application.DTOs.AccessControl;

/// <summary>
/// DTO de respuesta estándar para el endpoint /api/auth/me
/// Implementa el formato híbrido con navegación y permisos agrupados
/// </summary>
public class UserMeResponseDto
{
    public bool Success { get; set; } = true;
    public UserMeDataDto Data { get; set; } = new();
}

public class UserMeDataDto
{
    // Información básica del usuario
    public UserBasicInfoDto User { get; set; } = new();

    // Roles del usuario
    public List<UserRoleDto> Roles { get; set; } = new();

    // Configuración personalizada del portal por tipo de usuario
    public UserTypeDto? PortalConfiguration { get; set; }
}

public class UserBasicInfoDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Avatar { get; set; }
}

public class ResourcePermissionsDto
{
    public bool Read { get; set; }
    public bool Create { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool Export { get; set; }
    public bool Import { get; set; }
}
