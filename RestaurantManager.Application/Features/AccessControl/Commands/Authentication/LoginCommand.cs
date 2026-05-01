using Microsoft.Extensions.Logging;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Tokens;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Authentication;

public class LoginCommand : ILoginCommand
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ITokenCommand _tokenCommand;
    private readonly IRepositoryBase<UserType> _userTypeRepository;
    private readonly ILogger<LoginCommand> _logger;

    public LoginCommand(
        IRepositoryBase<User> userRepository,
        ILogger<LoginCommand> logger,
        IRepositoryBase<UserType> userTypeRepository,
        ITokenCommand tokenCommand)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userTypeRepository = userTypeRepository;
        _tokenCommand = tokenCommand;
    }

    public async Task<Result<LoginResponseDto>> Login(LoginRequestDto autorizacion, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar usuario solo por email
            User? CurrentUser = await _userRepository.Find(x => x.Email == autorizacion.Email, cancellationToken);

            if (CurrentUser == null)
            {
                _logger.LogWarning("Login: user not found {Email}", autorizacion.Email);
                return Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.InvalidCredentials", "Credenciales inválidas"));
            }

            if (CurrentUser.Status != true)
            {
                _logger.LogWarning("Login: inactive user {Email}", autorizacion.Email);
                return Result.Failure<LoginResponseDto>(Error.Forbidden("Login.InactiveUser", "Usuario inactivo. Contacte al administrador."));
            }

            // Verificar si el usuario existe y la contraseña es correcta
            if (!string.IsNullOrEmpty(CurrentUser.Password))
            {
                bool isPasswordValid = BC.Verify(autorizacion.Password, CurrentUser.Password);
                if (isPasswordValid)
                {
                    CurrentUser.UserType = await _userTypeRepository.Find(x => x.Id == CurrentUser.UserTypeId, cancellationToken);
                    _logger.LogInformation("Login: success for user {Email}", autorizacion.Email);
                    string CurrentToken = await _tokenCommand.GetToken(CurrentUser, cancellationToken);
                    LoginResponseDto loginResponse = new()
                    { Token = CurrentToken };

                    return Result.Success(loginResponse);
                }
                else
                {
                    _logger.LogWarning("Login: invalid password for user {Email}", autorizacion.Email);
                    return Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.InvalidCredentials", "Credenciales inválidas"));
                }
            }

            _logger.LogWarning("Login: user has no password {Email}", autorizacion.Email);
            return Result.Failure<LoginResponseDto>(Error.Unauthorized("Login.InvalidCredentials", "Credenciales inválidas"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login: unexpected error for user {Email}", autorizacion.Email);
            return Result.Failure<LoginResponseDto>(Error.Failure("Login.Error", "Error al intentar iniciar sesión"));
        }
    }
}
