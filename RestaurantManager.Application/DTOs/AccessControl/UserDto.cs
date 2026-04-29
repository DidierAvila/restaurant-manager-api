using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class UserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Image { get; set; }
    public string? Phone { get; set; }
    public Guid UserTypeId { get; set; }
    public string? UserTypeName { get; set; }
    public string? Address { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Status { get; set; }
    
    /// <summary>
    /// Roles asignados al usuario
    /// </summary>
    public List<RoleDropdownDto>? Roles { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public required string Name { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    public string? Password { get; set; }
    
    public string? Image { get; set; }
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "UserTypeId is required")]
    public Guid UserTypeId { get; set; }

    public string? Address { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    
    /// <summary>
    /// Lista de IDs de roles para asignar al usuario durante la creación
    /// </summary>
    public List<Guid>? RoleIds { get; set; }
    public bool Status { get; set; }
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? Phone { get; set; }
    public Guid? UserTypeId { get; set; }
    public string? Address { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    
    /// <summary>
    /// Lista de IDs de roles a asignar al usuario. 
    /// Si se proporciona, reemplaza todos los roles actuales del usuario.
    /// </summary>
    public List<Guid>? RoleIds { get; set; }
    public bool Status { get; set; }
}

public class UpdateCurrentUserDto
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
    public string? Phone { get; set; }
    public Guid? UserTypeId { get; set; }
    public string? Address { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class ResetPasswordDto
{
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    // En una implementación real, aquí tendríamos un token de reset
    public string? ResetToken { get; set; }
}

public class UserWithDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Image { get; set; }
    public string? Phone { get; set; }
    public Guid UserTypeId { get; set; }
    public string? Address { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Status { get; set; }
    public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();
    public UserTypeDto? UserType { get; set; }
}

public class UserSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Image { get; set; }
    public string? UserTypeName { get; set; }
    public string? Address { get; set; }
    public int RoleCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; } // Desde Sessions si es necesario
    public bool Status { get; set; } = true;
}

/// <summary>
/// DTO optimizado para listas de usuarios sin AdditionalData (mejor rendimiento)
/// </summary>
public class UserBasicDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Image { get; set; }
    public string? FirstRoleName { get; set; }
    public string? Phone { get; set; }
    public Guid UserTypeId { get; set; }
    public string? UserTypeName { get; set; }
    public string? Address { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Status { get; set; }
}
