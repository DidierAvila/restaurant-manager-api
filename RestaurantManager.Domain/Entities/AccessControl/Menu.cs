using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table("Menus", Schema = "accesscontrol")]
public class Menu
{
    [Key]
    public Guid Id { get; set; }

    public required string Label { get; set; }
    public required string Icon { get; set; }
    public required string Route { get; set; }
    public required int Order { get; set; }
    public required Boolean IsGroup { get; set; }
    public Guid? ParentId { get; set; }
    public required Boolean Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
