using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

public class UpdateUserAdditionalData
{
    private readonly IRepositoryBase<User> _userRepository;

    public UpdateUserAdditionalData(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserAdditionalValueResponseDto>> SetAdditionalValue(
        Guid userId,
        UserAdditionalDataOperationDto operationDto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            return Result.Failure<UserAdditionalValueResponseDto>(Error.NotFound("User.NotFound", "User not found"));

        user.SetAdditionalValue(operationDto.Key, operationDto.Value);
        await _userRepository.Update(user, cancellationToken);

        return Result.Success(new UserAdditionalValueResponseDto
        {
            Key = operationDto.Key,
            Value = operationDto.Value,
            Exists = true
        });
    }

    public async Task<Result<UserAdditionalValueResponseDto>> GetAdditionalValue(
        Guid userId,
        string key,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            return Result.Failure<UserAdditionalValueResponseDto>(Error.NotFound("User.NotFound", "User not found"));

        var exists = user.HasAdditionalValue(key);
        var value = exists ? user.GetAdditionalValue<object>(key) : null;

        return Result.Success(new UserAdditionalValueResponseDto
        {
            Key = key,
            Value = value,
            Exists = exists
        });
    }

    public async Task<Result> RemoveAdditionalValue(
        Guid userId,
        string key,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            return Result.Failure(Error.NotFound("User.NotFound", "User not found"));

        var removed = user.RemoveAdditionalValue(key);
        if (removed)
        {
            await _userRepository.Update(user, cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result<Dictionary<string, object>>> UpdateAllAdditionalData(
        Guid userId,
        UpdateUserAdditionalDataDto updateDto,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.Find(x => x.Id == userId, cancellationToken);
        if (user == null)
            return Result.Failure<Dictionary<string, object>>(Error.NotFound("User.NotFound", "User not found"));

        user.AdditionalData = updateDto.AdditionalData;
        await _userRepository.Update(user, cancellationToken);

        return Result.Success(user.AdditionalData);
    }
}
