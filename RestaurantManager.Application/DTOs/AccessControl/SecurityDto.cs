
using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Application.DTOs.AccessControl;

public class LoginRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponseDto
{
    public string? Token { get; set; }
}

public class ExternalLoginRequest
{
    /// <summary>
    /// id_token de Google obtenido en el frontend tras autenticarse.
    /// </summary>
    public string IdToken { get; set; } = null!;

    /// <summary>
    /// Proveedor de autenticación. Por ahora solo 'google'.
    /// </summary>
    public string Provider { get; set; } = "google";

    /// <summary>
    /// Tipo de usuario a crear cuando no exista en el sistema.
    /// Si no se envía, se usará el tipo por defecto.
    /// </summary>
    public UserTypeEnum? UserType { get; set; }
}
