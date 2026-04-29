using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Authentication;
using RestaurantManager.Application.Features.AccessControl.Commands.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Utils;

namespace RestaurantManager.Api.Controllers.AccessControl
{
    /// <summary>
    /// Controlador para la autenticación de usuarios.
    /// </summary>
    [ApiController]
    [Route("Api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ILoginCommand _loginCommand;
        private readonly IUserMeQueryHandler _userMeQueryHandler;
        private readonly IUserCommandHandler _userCommandHandler;

        /// <summary>
        /// Constructor de la clase AuthController.
        /// </summary>
        /// <param name="loginCommand">Comando para manejar el inicio de sesión.</param>
        /// <param name="logger">Instancia del logger para registrar información.</param>
        /// <param name="userMeQueryHandler">Handler para obtener información del usuario autenticado.</param>
        /// <param name="userCommandHandler">Handler para manejar comandos relacionados con usuarios.</param>
        public AuthController(
            ILoginCommand loginCommand,
            ILogger<AuthController> logger,
            IUserMeQueryHandler userMeQueryHandler,
            IUserCommandHandler userCommandHandler)
            => (_loginCommand, _logger, _userMeQueryHandler, _userCommandHandler) =
                (loginCommand, logger, userMeQueryHandler, userCommandHandler);

        /// <summary>
        /// Inicia sesión en el sistema con las credenciales proporcionadas.
        /// </summary>
        /// <param name="autorizacion">Objeto que contiene las credenciales de inicio de sesión (email y contraseña).</param>
        /// <param name="cancellationToken">Token de cancelación para la operación asíncrona.</param>
        /// <returns>Un token JWT si las credenciales son válidas; de lo contrario, un error.</returns>
        /// <response code="200">Retorna el token JWT de autenticación</response>
        /// <response code="400">Credenciales inválidas o datos de entrada incorrectos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto autorizacion, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login");
            LoginResponseDto? Response = await _loginCommand.Login(autorizacion, cancellationToken);

            if (Response == null)
                return BadRequest();

            return Ok(Response.Token);
        }

        /// <summary>
        /// Obtiene la información del usuario autenticado en formato híbrido con navegación y permisos agrupados.
        /// Formato optimizado para frontends con navegación dinámica y permisos granulares.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación para la operación asíncrona.</param>
        /// <returns>Información completa del usuario autenticado incluyendo roles, permisos y navegación.</returns>
        /// <response code="200">Retorna la información del usuario autenticado</response>
        /// <response code="401">Usuario no autenticado o token inválido</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [Route("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserMeResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserMeResponseDto>> GetMe(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetMe endpoint called");

                // Obtener el ID del usuario desde el token JWT
                var userIdClaim = User.FindFirst(CustomClaimTypes.UserId)?.Value;
                _logger.LogInformation("User ID claim from token: {UserId}", userIdClaim);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID in token");
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });
                }

                _logger.LogInformation("Parsed user ID: {UserId}", userId);

                // Obtener la información del usuario en formato híbrido
                var userMeHybrid = await _userMeQueryHandler.GetUserMe(userId, cancellationToken);

                _logger.LogInformation("Successfully retrieved hybrid user info for user ID: {UserId}", userId);
                return Ok(userMeHybrid);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User not found in database");
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user information");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza los datos personales del usuario autenticado
        /// </summary>
        /// <param name="updateDto">Datos a actualizar del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Usuario actualizado</returns>
        /// <response code="200">Retorna el usuario actualizado</response>
        /// <response code="401">Usuario no autenticado o token inválido</response>
        /// <response code="404">Usuario no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPut]
        [Route("me/update")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDto>> UpdateCurrentUser([FromBody] UpdateCurrentUserDto updateDto, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UpdateCurrentUser endpoint called");

                // Obtener el ID del usuario desde el token JWT
                var userIdClaim = User.FindFirst(CustomClaimTypes.UserId)?.Value;
                _logger.LogInformation("User ID claim from token: {UserId}", userIdClaim);

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogWarning("Invalid or missing user ID in token");
                    return Unauthorized(new { message = "Invalid or missing user ID in token" });
                }

                _logger.LogInformation("Parsed user ID: {UserId}", userId);

                // Crear el DTO para actualizar el usuario
                var updateUserDto = new UpdateUserDto
                {
                    Name = updateDto.Name,
                    Email = updateDto.Email,
                    Phone = updateDto.Phone,
                    Image = updateDto.Image,
                    Address = updateDto.Address,
                    UserTypeId = updateDto.UserTypeId,
                    AdditionalData = updateDto.AdditionalData
                };

                // Llamar al servicio existente de actualización de usuarios
                var updatedUser = await _userCommandHandler.UpdateUser(userId, updateUserDto, cancellationToken);

                _logger.LogInformation("Successfully updated user with ID: {UserId}", userId);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "User not found in database");
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user information");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }
    }
}
