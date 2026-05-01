using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Roles;

public class DeleteRole
{
    private readonly IRepositoryBase<Role> _roleRepository;

    public DeleteRole(IRepositoryBase<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result> HandleAsync(Guid id, CancellationToken cancellationToken)
    {
        // Find existing role
        var role = await _roleRepository.Find(x => x.Id == id, cancellationToken);
        if (role == null)
            return Result.Failure(Error.NotFound("Role.NotFound", "Role not found"));

        // Delete role from repository
        await _roleRepository.Delete(role, cancellationToken);

        return Result.Success();
    }
}
