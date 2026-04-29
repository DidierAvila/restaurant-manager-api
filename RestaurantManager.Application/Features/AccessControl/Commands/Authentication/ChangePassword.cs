using RestaurantManager.Application.DTOs.AccessControl;
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

    public async Task<bool> HandleAsync(Guid userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        // Validations
        if (string.IsNullOrWhiteSpace(changePasswordDto.CurrentPassword))
            throw new ArgumentException("Current password is required");

        if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
            throw new ArgumentException("New password is required");

        if (changePasswordDto.NewPassword.Length < 6)
            throw new ArgumentException("New password must be at least 6 characters long");

        // Find user
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User not found");

        // Verify current password
        if (string.IsNullOrEmpty(user.Password) || !BC.Verify(changePasswordDto.CurrentPassword, user.Password))
            throw new UnauthorizedAccessException("Current password is incorrect");

        // Hash new password and update
        user.Password = BC.HashPassword(changePasswordDto.NewPassword, 12);
        user.UpdatedAt = DateTime.Now;

        await _userRepository.Update(user, cancellationToken);
        return true;
    }
}
