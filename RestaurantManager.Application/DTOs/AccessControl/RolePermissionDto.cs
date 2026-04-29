using System.ComponentModel.DataAnnotations;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class RolePermissionDto
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public string? RoleName { get; set; }
    public string? PermissionName { get; set; }
}

public class CreateRolePermissionDto
{
    [Required]
    public Guid RoleId { get; set; }

    [Required]
    public Guid PermissionId { get; set; }
}

public class AssignPermissionToRoleDto
{
    [Required]
    public Guid RoleId { get; set; }

    [Required]
    public Guid PermissionId { get; set; }
}

public class RoleWithPermissionsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Status { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
}

/// <summary>
/// DTO para asignar múltiples permisos a un rol de una vez
/// </summary>
public class AssignMultiplePermissionsToRoleDto
{
    [Required]
    public Guid RoleId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Debe especificar al menos un permiso")]
    public List<Guid> PermissionIds { get; set; } = new();
}

/// <summary>
/// Resultado de la asignación múltiple de permisos
/// </summary>
public class MultiplePermissionAssignmentResult
{
    public Guid RoleId { get; set; }
    public string? RoleName { get; set; }
    public List<RolePermissionDto> SuccessfulAssignments { get; set; } = new();
    public List<string> ExistingPermissions { get; set; } = new();
    public List<PermissionAssignmentError> FailedAssignments { get; set; } = new();
    
    public int TotalProcessed => SuccessfulAssignments.Count + ExistingPermissions.Count + FailedAssignments.Count;
    public bool HasErrors => FailedAssignments.Any();
}

public class PermissionAssignmentError
{
    public Guid PermissionId { get; set; }
    public string? PermissionName { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
