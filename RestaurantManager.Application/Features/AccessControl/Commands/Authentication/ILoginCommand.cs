using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Authentication;

public interface ILoginCommand
{
    Task<Result<LoginResponseDto>> Login(LoginRequestDto autorizacion, CancellationToken cancellationToken);
}
