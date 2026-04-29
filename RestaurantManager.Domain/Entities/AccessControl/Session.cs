using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantManager.Domain.Entities.AccessControl;

[Table(name: "Session", Schema = "accesscontrol")]
public partial class Session
{
    public Guid Id { get; set; }
    public string SessionToken { get; set; } = null!;
    public Guid? UserId { get; set; }
    public DateTime Expires { get; set; }
    public virtual User? User { get; set; }
}
