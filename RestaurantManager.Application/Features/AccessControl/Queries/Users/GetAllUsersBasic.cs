using AutoMapper;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Users;

public class GetAllUsersBasic
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersBasic(IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserBasicDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAll(cancellationToken);

        // Map collection of Entities to DTOs using AutoMapper (sin AdditionalData)
        var userDtos = _mapper.Map<IEnumerable<UserBasicDto>>(users);

        // Asignar UserTypeName manualmente
        foreach (var userDto in userDtos)
        {
            userDto.UserTypeName = "Admin";
        }

        return userDtos;
    }
}
