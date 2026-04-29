using RestaurantManager.Domain.Entities.AccessControl;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Tokens;

public interface ITokenCommand
{
    Task<string> GetToken(User user, CancellationToken cancellationToken);
}