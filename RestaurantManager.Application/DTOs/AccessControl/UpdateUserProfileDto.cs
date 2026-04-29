namespace RestaurantManager.Application.DTOs.AccessControl;

public class UpdateUserProfileDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Image { get; set; }
    public UserConfigurationDto? Configuration { get; set; }
    public Dictionary<string, object>? AdditionalData { get; set; }
}
