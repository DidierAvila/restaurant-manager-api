namespace RestaurantManager.Application.DTOs.AccessControl;

/// <summary>
/// DTO para manejar la actualización de datos adicionales de un usuario
/// </summary>
public class UpdateUserAdditionalDataDto
{
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

/// <summary>
/// DTO para operaciones específicas sobre datos adicionales
/// </summary>
public class UserAdditionalDataOperationDto
{
    public string Key { get; set; } = null!;
    public object? Value { get; set; }
}

/// <summary>
/// DTO para obtener un valor específico de los datos adicionales
/// </summary>
public class GetUserAdditionalValueDto
{
    public string Key { get; set; } = null!;
}

/// <summary>
/// DTO de respuesta para valores de datos adicionales
/// </summary>
public class UserAdditionalValueResponseDto
{
    public string Key { get; set; } = null!;
    public object? Value { get; set; }
    public bool Exists { get; set; }
}
