using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table("UserRoles", Schema = "accesscontrol")]
public class UserRole
{
    [Key]
    [Column(Order = 0)]
    public Guid RoleId { get; set; }

    [Key]
    [Column(Order = 1)]
    public Guid UserId { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
