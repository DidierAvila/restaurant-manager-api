using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Authentication;

public class ChangePassword
{
    private readonly IRepositoryBase<User> _userRepository;

    public ChangePassword(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> HandleAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(changePasswordDto.CurrentPassword))
            return Result.Failure(Error.Validation("Password.CurrentRequired", "Current password is required"));

        if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
            return Result.Failure(Error.Validation("Password.NewRequired", "New password is required"));

        if (changePasswordDto.NewPassword.Length < 6)
            return Result.Failure(Error.Validation("Password.TooShort", "New password must be at least 6 characters long"));

        // Find user
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

        // Verify current password
        if (string.IsNullOrEmpty(user.Password) || !BC.Verify(changePasswordDto.CurrentPassword, user.Password))
            return Result.Failure(Error.Unauthorized("Password.Incorrect", "Current password is incorrect"));

        // Hash new password and update
        user.Password = BC.HashPassword(changePasswordDto.NewPassword, 12);
        user.UpdatedAt = DateTime.Now;

        await _userRepository.Update(user, cancellationToken);
        return Result.Success();
    }
}
