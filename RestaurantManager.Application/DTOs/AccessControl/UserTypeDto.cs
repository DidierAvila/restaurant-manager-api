namespace RestaurantManager.Application.DTOs.AccessControl;

public class UserTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Status { get; set; }
    // Campos migrados desde UserTypePortalConfig
    public string Theme { get; set; } = "default";
    public string? DefaultLandingPage { get; set; }
    public string? LogoUrl { get; set; }
    public string Language { get; set; } = "es";
    public System.Text.Json.JsonElement? AdditionalConfig { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateUserTypeDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Status { get; set; } = true;
    // Campos migrados desde UserTypePortalConfig
    public string Theme { get; set; } = "default";
    public string? DefaultLandingPage { get; set; }
    public string? LogoUrl { get; set; }
    public string Language { get; set; } = "es";
    public Dictionary<string, object>? AdditionalConfig { get; set; }
}

public class UpdateUserTypeDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool Status { get; set; }
    // Campos migrados desde UserTypePortalConfig
    public string Theme { get; set; } = "default";
    public string? DefaultLandingPage { get; set; }
    public string? LogoUrl { get; set; }
    public string Language { get; set; } = "es";
    public Dictionary<string, object>? AdditionalConfig { get; set; }
}

public class UserTypeSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Status { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO optimizado para dropdowns/listas desplegables de tipos de usuario (máximo rendimiento)
/// </summary>
public class UserTypeDropdownDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
