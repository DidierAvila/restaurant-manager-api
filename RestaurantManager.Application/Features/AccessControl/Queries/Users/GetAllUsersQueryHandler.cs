using AutoMapper;
using MediatR;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;

namespace RestaurantManager.Application.Features.AccessControl.Queries.Users;

/// <summary>
/// Handler para la query GetAllUsersQuery
/// </summary>
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(
        IRepositoryBase<User> userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAll(cancellationToken);

        // Map collection of Entities to DTOs using AutoMapper
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        // Asignar UserTypeName manualmente
        foreach (var userDto in userDtos)
        {
            userDto.UserTypeName = "Admin";
        }

        return userDtos;
    }
}
