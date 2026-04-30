using System;
using System.Collections.Generic;

namespace RestaurantManager.Api.ScaffoldedModels;

public partial class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string? Image { get; set; }

    public string? Phone { get; set; }

    public Guid UserTypeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string ExtraData { get; set; } = null!;

    public bool? Status { get; set; }

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual UserType UserType { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
