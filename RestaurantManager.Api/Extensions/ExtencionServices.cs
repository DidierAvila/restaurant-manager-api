using RestaurantManager.Application.Core.Auth.Commands.RolePermissions;
using RestaurantManager.Application.Core.Auth.Queries.Handlers;
using RestaurantManager.Application.Core.Auth.Queries.RolePermissions;
using RestaurantManager.Application.Features.AccessControl.Commands.Authentication;
using RestaurantManager.Application.Features.AccessControl.Commands.Handlers;
using RestaurantManager.Application.Features.AccessControl.Commands.Permissions;
using RestaurantManager.Application.Features.AccessControl.Commands.RolePermissions;
using RestaurantManager.Application.Features.AccessControl.Commands.Roles;
using RestaurantManager.Application.Features.AccessControl.Commands.Tokens;
using RestaurantManager.Application.Features.AccessControl.Commands.Users;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Permissions;
using RestaurantManager.Application.Features.AccessControl.Queries.RolePermissions;
using RestaurantManager.Application.Features.AccessControl.Queries.Roles;
using RestaurantManager.Application.Features.AccessControl.Queries.UserMe;
using RestaurantManager.Application.Features.AccessControl.Queries.Users;
using RestaurantManager.Domain.Entities.AccessControl;
using RestaurantManager.Domain.Repositories;
using RestaurantManager.Domain.Repositories.AccessControl;
using RestaurantManager.Infrastructure.Repositories;
using RestaurantManager.Infrastructure.Repositories.Auth;

namespace RestaurantManager.Api.Extensions;

public static class ExtencionServices
{
    public static IServiceCollection AddApiExtention(this IServiceCollection services)
    {
        // AutoMapper
        //services.AddAutoMapper(typeof(UserProfile), typeof(UserTypeProfile), typeof(PermissionProfile), typeof(AuthProfile), typeof(RoleProfile), typeof(RolePermissionProfile), typeof(MenuProfile), typeof(MenuPermissionProfile), typeof(UserRelationshipProfile));

        // Authentication Commands
        services.AddScoped<ILoginCommand, LoginCommand>();
        services.AddScoped<ITokenCommand, TokenCommand>();

        // User Commands
        services.AddScoped<CreateUser>();
        services.AddScoped<UpdateUser>();
        services.AddScoped<DeleteUser>();
        services.AddScoped<ChangePassword>();
        services.AddScoped<UpdateUserAdditionalData>();
        services.AddScoped<RemoveMultipleRolesFromUser>();
        services.AddScoped<IUserCommandHandler, UserCommandHandler>();

        // User Queries  
        services.AddScoped<GetUserById>();
        services.AddScoped<GetAllUsers>();
        services.AddScoped<GetAllUsersBasic>();
        services.AddScoped<GetAllUsersFiltered>();
        services.AddScoped<IUserQueryHandler, UserQueryHandler>();

        // Permission Commands
        services.AddScoped<CreatePermission>();
        services.AddScoped<UpdatePermission>();
        services.AddScoped<DeletePermission>();
        services.AddScoped<IPermissionCommandHandler, PermissionCommandHandler>();

        // Permission Queries
        services.AddScoped<GetPermissionById>();
        services.AddScoped<GetAllPermissions>();
        services.AddScoped<GetActivePermissions>();
        services.AddScoped<GetPermissionsSummary>();
        services.AddScoped<GetAllPermissionsFiltered>();
        services.AddScoped<GetPermissionsForDropdown>();
        services.AddScoped<IPermissionQueryHandler, PermissionQueryHandler>();

        // Role Commands
        services.AddScoped<CreateRole>();
        services.AddScoped<UpdateRole>();
        services.AddScoped<DeleteRole>();
        services.AddScoped<RemoveMultiplePermissionsFromRole>();
        services.AddScoped<IRoleCommandHandler, RoleCommandHandler>();

        // Role Queries  
        services.AddScoped<GetRoleById>();
        services.AddScoped<GetAllRoles>();
        services.AddScoped<GetRolesDropdown>();
        services.AddScoped<GetAllRolesFiltered>();
        services.AddScoped<IRoleQueryHandler, RoleQueryHandler>();

        // RolePermission Commands
        services.AddScoped<AssignPermissionToRole>();
        services.AddScoped<AssignMultiplePermissionsToRole>();
        services.AddScoped<RemovePermissionFromRole>();

        // RolePermission Queries
        services.AddScoped<GetAllRolePermissions>();
        services.AddScoped<GetRolesByPermission>();
        services.AddScoped<GetPermissionsByRole>();

        // UserMe Queries
        services.AddScoped<GetUserMe>();
        services.AddScoped<IUserMeQueryHandler, UserMeQueryHandler>();

        // Repositories
        services.AddScoped<IRepositoryBase<User>, RepositoryBase<User>>();
        services.AddScoped<IRepositoryBase<Session>, RepositoryBase<Session>>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRepositoryBase<Role>, RepositoryBase<Role>>();
        services.AddScoped<IRepositoryBase<Permission>, RepositoryBase<Permission>>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRepositoryBase<UserRole>, RepositoryBase<UserRole>>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();

        return services;
    }
}
