using System;
using System.Collections.Generic;

namespace RestaurantManager.Api.ScaffoldedModels;

public partial class UserType
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Theme { get; set; }

    public string? DefaultLandingPage { get; set; }

    public string? LogoUrl { get; set; }

    public string? Language { get; set; }

    public string? AdditionalConfig { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
