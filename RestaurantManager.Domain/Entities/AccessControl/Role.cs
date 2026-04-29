using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table("Roles", Schema = "accesscontrol")]
public partial class Role
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public required bool Status { get; set; }

    public required DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = [];

    public virtual ICollection<User> Users { get; set; } = [];

    // Helper property para acceso directo a permisos
    [NotMapped]
    public virtual ICollection<Permission> Permissions => [.. RolePermissions.Select(rp => rp.Permission)];
}
