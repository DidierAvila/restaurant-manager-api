using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RestaurantManager.Application.Utils;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Tokens;

public class TokenCommand : ITokenCommand
{
    private readonly IRepositoryBase<Session> _sessionRepository;
    private readonly ILogger<TokenCommand> _logger;
    private readonly IConfiguration _configuration;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRepositoryBase<Permission> _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    public TokenCommand(
        IRepositoryBase<Session> sessionRepository,
        IConfiguration configuration,
        IRolePermissionRepository rolePermissionRepository,
        IRepositoryBase<Permission> permissionRepository,
        IUserRoleRepository userRoleRepository,
        ILogger<TokenCommand> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
        _configuration = configuration;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
    }

    public async Task<string> GetToken(User user, CancellationToken cancellationToken)
    {
        var CurrentSession = await _sessionRepository.Find(x => x.UserId == user.Id && x.Expires > DateTime.Now, cancellationToken);
        if (CurrentSession != null)
        {
            if (CurrentSession.Expires.CompareTo(DateTime.Now) < 0)
            {
                _logger.LogInformation("GetToken: Expiration Session UserId:" + user.Id);
                return await RefreshSessionAsync(CurrentSession, user, cancellationToken);
            }
            return CurrentSession.SessionToken!;
        }
        else
        {
            string currentToken = await GenerateTokenAsync(user, cancellationToken);
            if (!string.IsNullOrEmpty(currentToken))
            {
                return currentToken;
            }
        }
        return string.Empty;
    }

    private async Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken)
    {
        string? key = _configuration.GetValue<string>("JwtSettings:key");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Obtener permisos del usuario
        List<string> userPermissions = await GetUserPermissionsAsync(user.Id, cancellationToken);

        // Crear los claims básicos
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.UserId, user.Id.ToString()),
            new(CustomClaimTypes.UserName, user.Name),
            new(CustomClaimTypes.UserEmail, user.Email),
            new(CustomClaimTypes.UserTypeId, user.UserTypeId.ToString()),
            new(CustomClaimTypes.UserTypeName, user.UserType!),
        };

        // Agregar permisos como claims
        foreach (var permission in userPermissions)
        {
            claims.Add(new Claim(CustomClaimTypes.Permission, permission));
        }

        // Crear el token
        DateTime ExperiredDate = DateTime.Now.AddMinutes(60);
        JwtSecurityToken tokenJwt = new(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: ExperiredDate,
            signingCredentials: credentials
        );

        string Newtoken = new JwtSecurityTokenHandler().WriteToken(tokenJwt);

        //Se almacena el nuevo token como session
        if (!string.IsNullOrEmpty(Newtoken))
        {
            Session session = new()
            {
                Id = Guid.NewGuid(),
                SessionToken = Newtoken,
                UserId = user.Id,
                Expires = ExperiredDate
            };

            await _sessionRepository.Create(session, cancellationToken);
        }
        return Newtoken;
    }

    private async Task<string> RefreshSessionAsync(Session session, User user, CancellationToken cancellationToken)
    {
        // Eliminar sesión anterior
        await _sessionRepository.Delete(session.Id.GetHashCode(), cancellationToken);

        string currentToken = await GenerateTokenAsync(user, cancellationToken);
        return currentToken;
    }

    /// <summary>
    /// Obtiene todos los permisos del usuario basado en sus roles
    /// </summary>
    private async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var permissions = new HashSet<string>();

        // Obtener roles del usuario
        var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, cancellationToken);

        // Obtener permisos de cada rol
        foreach (var userRole in userRoles)
        {
            var rolePermissions = await _rolePermissionRepository.GetPermissionsByRoleIdAsync(userRole.RoleId, cancellationToken);

            foreach (var rolePermission in rolePermissions)
            {
                var permission = await _permissionRepository.Find(p => p.Id == rolePermission.PermissionId, cancellationToken);
                if (permission != null && permission.Status)
                {
                    permissions.Add(permission.Name);
                }
            }
        }

        return [.. permissions];
    }
}
