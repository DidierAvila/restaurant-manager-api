namespace RestaurantManager.Application.DTOs.AccessControl;

public class SessionDto
{
    public Guid Id { get; set; }
    public string SessionToken { get; set; } = null!;
    public Guid? UserId { get; set; }
    public DateTime Expires { get; set; }
    public UserDto? User { get; set; }
}

public class CreateSessionDto
{
    public string SessionToken { get; set; } = null!;
    public Guid UserId { get; set; }
    public DateTime Expires { get; set; }
}
