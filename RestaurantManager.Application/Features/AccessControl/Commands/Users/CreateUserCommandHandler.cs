using AutoMapper;
using MediatR;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Domain.Common;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;
using BC = BCrypt.Net.BCrypt;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

/// <summary>
/// Handler para el comando CreateUserCommand
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Role> _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IRepositoryBase<User> userRepository,
        IRepositoryBase<Role> roleRepository,
        IUserRoleRepository userRoleRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var createUserDto = request.UserDto;

        // Validations
        if (string.IsNullOrWhiteSpace(createUserDto.Email))
            return Result.Failure<UserDto>(Error.Validation("User.EmailRequired", "Email is required"));

        if (createUserDto.UserTypeId == Guid.Empty)
            return Result.Failure<UserDto>(Error.Validation("User.UserTypeRequired", "UserTypeId is required"));

        // Check if user already exists
        var existingUser = await _userRepository.Find(x => x.Email == createUserDto.Email, cancellationToken);
        if (existingUser != null)
            return Result.Failure<UserDto>(Error.Conflict("User.EmailExists", "User with this email already exists"));

        // Map DTO to Entity using AutoMapper
        var user = _mapper.Map<User>(createUserDto);

        // Encrypt password before saving
        if (!string.IsNullOrEmpty(user.Password))
        {
            user.Password = BC.HashPassword(user.Password, 12);
        }

        // Create user in repository
        var createdUser = await _userRepository.Create(user, cancellationToken);

        // Asignar roles si se proporcionaron
        if (createUserDto.RoleIds != null && createUserDto.RoleIds.Any())
        {
            await AssignRolesToUser(createdUser.Id, createUserDto.RoleIds, cancellationToken);
        }

        // Map Entity to DTO using AutoMapper
        var userDto = _mapper.Map<UserDto>(createdUser);
        userDto.UserTypeName = "Admin";

        // Cargar roles asignados para incluir en la respuesta
        if (createUserDto.RoleIds != null && createUserDto.RoleIds.Any())
        {
            userDto.Roles = await LoadUserRoles(createdUser.Id, cancellationToken);
        }

        return Result.Success(userDto);
    }

    /// <summary>
    /// Asigna múltiples roles a un usuario recién creado
    /// </summary>
    private async Task AssignRolesToUser(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken)
    {
        // Validar que todos los roles existen
        var validRoles = new List<Role>();
        foreach (var roleId in roleIds)
        {
            var role = await _roleRepository.Find(r => r.Id == roleId && r.Status, cancellationToken);
            if (role != null)
            {
                validRoles.Add(role);
            }
        }

        // Asignar los roles válidos al usuario
        if (validRoles.Any())
        {
            foreach (var role in validRoles)
            {
                await _userRoleRepository.AssignRoleToUserAsync(userId, role.Id, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Carga los roles del usuario para incluir en la respuesta
    /// </summary>
    private async Task<List<RoleDropdownDto>> LoadUserRoles(Guid userId, CancellationToken cancellationToken)
    {
        var userRoles = await _userRoleRepository.GetUserRolesWithDetailsAsync(userId, cancellationToken);

        return userRoles
            .Where(ur => ur.Role != null && ur.Role.Status)
            .Select(ur => new RoleDropdownDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name
            })
            .ToList();
    }
}
