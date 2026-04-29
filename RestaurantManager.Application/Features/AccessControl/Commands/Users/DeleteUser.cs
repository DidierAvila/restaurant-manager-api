using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

public class DeleteUser
{
    private readonly IRepositoryBase<User> _userRepository;

    public DeleteUser(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        // Find existing user
        var user = await _userRepository.Find(x => x.Id == id, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        await _userRepository.Delete(user, cancellationToken);
        return true;
    }
}
