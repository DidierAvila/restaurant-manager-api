using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManager.Api.Attributes;
using RestaurantManager.Application.DTOs.AccessControl;
using RestaurantManager.Application.DTOs.Common;
using RestaurantManager.Application.Features.AccessControl.Commands.Handlers;
using RestaurantManager.Application.Features.AccessControl.Commands.Users;
using RestaurantManager.Application.Features.AccessControl.Queries.Handlers;
using RestaurantManager.Application.Features.AccessControl.Queries.Users;

namespace RestaurantManager.Api.Controllers.AccessControl;

/// <summary>
/// Controlador para gestionar los usuarios del sistema.
/// </summary>
[Route("Api/Auth/[controller]")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserCommandHandler _userCommandHandler;
    private readonly IUserQueryHandler _userQueryHandler;

    /// <summary>
    /// Constructor del controlador UsersController
    /// </summary>
    /// <param name="userCommandHandler"></param>
    /// <param name="userQueryHandler"></param>
    /// <param name="logger"></param>
    public UsersController(IUserCommandHandler userCommandHandler, IUserQueryHandler userQueryHandler, ILogger<UsersController> logger)
    {
        _userCommandHandler = userCommandHandler;
        _userQueryHandler = userQueryHandler;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene una lista paginada de usuarios con filtros opcionales
    /// </summary>
    /// <param name="filter">Filtros de búsqueda y paginación</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista paginada de usuarios</returns>
    /// <remarks>
    /// Campos disponibles para SortBy: name, email, username, createdat, usertypeid
    ///
    /// Ejemplo de uso:
    /// GET /api/users?page=1&amp;pageSize=10&amp;search=juan&amp;sortBy=name
    /// </remarks>
    [HttpGet]
    [RequirePermission("users.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] UserFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _userQueryHandler.GetAllUsersFiltered(filter, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un usuario por su ID incluyendo sus roles asignados
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario con roles asignados</returns>
    [HttpGet("{id}")]
    [RequirePermission("users.read")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userQueryHandler.GetUserById(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Crea un nuevo usuario con roles opcionales
    /// </summary>
    /// <param name="createDto">Datos del usuario a crear, incluyendo roles opcionales</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario creado con roles asignados</returns>
    [HttpPost]
    [RequirePermission("users.create")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto createDto, CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.CreateUser(createDto, cancellationToken);

        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return HandleError(result.Error);
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="updateDto">Datos a actualizar del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Usuario actualizado con roles asignados</returns>
    /// <remarks>
    /// La respuesta incluye los roles actuales del usuario después de la actualización.
    /// Para gestionar roles específicamente, usar los endpoints de gestión de roles
    /// </remarks>
    [HttpPut("{id}")]
    [RequirePermission("users.update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto updateDto, CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.UpdateUser(id, updateDto, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un usuario por su ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [RequirePermission("users.delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.DeleteUser(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cambia la contraseña de un usuario
    /// </summary>
    /// <param name="id"></param>
    /// <param name="changePasswordDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id}/change-password")]
    [RequirePermission("users.update")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.ChangePassword(id, changePasswordDto, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Establece un valor en los datos adicionales del usuario
    /// </summary>
    [HttpPut("{id}/additional-data/{key}")]
    [RequirePermission("users.update")]
    public async Task<IActionResult> SetAdditionalValue(
        Guid id,
        string key,
        [FromBody] object value,
        CancellationToken cancellationToken)
    {
        var operationDto = new UserAdditionalDataOperationDto { Key = key, Value = value };
        var result = await _userCommandHandler.SetUserAdditionalValue(id, operationDto, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Obtiene un valor específico de los datos adicionales del usuario
    /// </summary>
    [HttpGet("{id}/additional-data/{key}")]
    [RequirePermission("users.read")]
    public async Task<IActionResult> GetAdditionalValue(
        Guid id,
        string key,
        CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.GetUserAdditionalValue(id, key, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Elimina un valor de los datos adicionales del usuario
    /// </summary>
    [HttpDelete("{id}/additional-data/{key}")]
    [RequirePermission("users.update")]
    public async Task<IActionResult> RemoveAdditionalValue(
        Guid id,
        string key,
        CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.RemoveUserAdditionalValue(id, key, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Actualiza todos los datos adicionales del usuario
    /// </summary>
    [HttpPut("{id}/additional-data")]
    [RequirePermission("users.update")]
    public async Task<IActionResult> UpdateAdditionalData(
        Guid id,
        [FromBody] UpdateUserAdditionalDataDto updateDto,
        CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.UpdateUserAdditionalData(id, updateDto, cancellationToken);
        return HandleResult(result);
    }

    // ======================================
    // ROLE MANAGEMENT ENDPOINTS
    // ======================================

    /// <summary>
    /// Obtiene todos los roles asignados a un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Lista de roles del usuario</returns>
    [HttpGet("{id}/roles")]
    [RequirePermission("users.read")]
    public async Task<IActionResult> GetUserRoles(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userCommandHandler.GetUserRoles(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Asigna múltiples roles a un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="roleIds">Lista de IDs de roles a asignar</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la asignación múltiple</returns>
    [HttpPost("{id}/roles")]
    [RequirePermission("users.manage")]
    public async Task<IActionResult> AssignRolesToUser(
        Guid id,
        [FromBody] List<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var command = new AssignMultipleRolesToUser
        {
            UserId = id,
            RoleIds = roleIds
        };

        var result = await _userCommandHandler.AssignMultipleRolesToUser(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remueve múltiples roles de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="roleIds">Lista de IDs de roles a remover</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    /// <returns>Resultado de la remoción múltiple</returns>
    [HttpDelete("{id}/roles")]
    [RequirePermission("users.manage")]
    public async Task<IActionResult> RemoveRolesFromUser(
        Guid id,
        [FromBody] List<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var command = new RemoveMultipleRolesFromUser
        {
            UserId = id,
            RoleIds = roleIds
        };

        var result = await _userCommandHandler.RemoveMultipleRolesFromUser(command, cancellationToken);
        return HandleResult(result);
    }
}
