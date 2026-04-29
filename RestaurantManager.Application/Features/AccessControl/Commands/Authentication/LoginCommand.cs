using Microsoft.Extensions.Logging;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.Features.AccessControl.Commands.Tokens;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Authentication;

public class LoginCommand : ILoginCommand
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ITokenCommand _tokenCommand;
    private readonly ILogger<LoginCommand> _logger;

    public LoginCommand(
        IRepositoryBase<User> userRepository, 
        ILogger<LoginCommand> logger, 
        ITokenCommand tokenCommand) 
    {
        _userRepository = userRepository;
        _logger = logger;
        _tokenCommand = tokenCommand;
    }

    public async Task<LoginResponseDto?> Login(LoginRequestDto autorizacion, CancellationToken cancellationToken)
    {
        // Buscar usuario solo por email
        User? CurrentUser = await _userRepository.Find(x => x.Email == autorizacion.Email, cancellationToken);

        if (CurrentUser == null || !CurrentUser!.Status)
        {
            throw new UnauthorizedAccessException("Usuario inactivo. Contacte al administrador.");
        }

        // Verificar si el usuario existe y la contraseña es correcta
        if (CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.Password))
        {
            bool isPasswordValid = BC.Verify(autorizacion.Password, CurrentUser.Password);
            if (isPasswordValid)
            {
                CurrentUser.UserType = "Admin";
                _logger.LogInformation("Login: success");
                string CurrentToken = await _tokenCommand.GetToken(CurrentUser, cancellationToken);
                LoginResponseDto loginResponse = new()
                { Token = CurrentToken };

                return loginResponse;
            }
            else
            {
                _logger.LogWarning("Login: invalid password for user {Email}", autorizacion.Email);
            }
        }
        else
        {
            _logger.LogWarning("Login: user not found {Email}", autorizacion.Email);
        }
        return null;
    }
}
