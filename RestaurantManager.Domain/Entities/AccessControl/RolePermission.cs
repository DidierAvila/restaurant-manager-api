using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table("RolePermissions", Schema = "accesscontrol")]
public class RolePermission
{
    [Key]
    [Column(Order = 0)]
    public Guid RoleId { get; set; }

    [Key]
    [Column(Order = 1)]
    public Guid PermissionId { get; set; }

    // Navigation properties
    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("PermissionId")]
    public virtual Permission Permission { get; set; } = null!;
}