using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table(name: "UserTypes", Schema = "accesscontrol")]
public partial class UserType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool Status { get; set; }

    // Campos migrados desde UserTypePortalConfig
    public string? Theme { get; set; } = "default";
    public string? DefaultLandingPage { get; set; }
    public string? LogoUrl { get; set; }
    public string? Language { get; set; } = "es";

    public string? AdditionalConfig { get; set; }

    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<User> Users { get; set; } = [];
}
