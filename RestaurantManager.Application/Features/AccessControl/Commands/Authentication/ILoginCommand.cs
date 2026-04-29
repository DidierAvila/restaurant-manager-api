using RestaurantManager.Application.DTOs.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Authentication;

public interface ILoginCommand
{
    Task<LoginResponseDto?> Login(LoginRequestDto autorizacion, CancellationToken cancellationToken);
}
