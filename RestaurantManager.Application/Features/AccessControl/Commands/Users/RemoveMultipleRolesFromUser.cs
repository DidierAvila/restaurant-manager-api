namespace RestaurantManager.Application.Features.AccessControl.Commands.Users;

/// <summary>
/// Comando para remover múltiples roles de un usuario
/// </summary>
public class RemoveMultipleRolesFromUser
{
    /// <summary>
    /// ID del usuario
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Lista de IDs de roles a remover
    /// </summary>
    public List<Guid> RoleIds { get; set; } = new();
}

/// <summary>
/// Resultado de la remoción múltiple de roles
/// </summary>
public class MultipleRoleRemovalResult
{
    /// <summary>
    /// Roles removidos exitosamente
    /// </summary>
    public List<Guid> RemovedRoles { get; set; } = new();

    /// <summary>
    /// Roles que no estaban asignados
    /// </summary>
    public List<Guid> NotAssignedRoles { get; set; } = new();

    /// <summary>
    /// Roles que no se pudieron remover (errores)
    /// </summary>
    public List<string> FailedRoles { get; set; } = new();

    /// <summary>
    /// Número total de roles procesados
    /// </summary>
    public int TotalProcessed => RemovedRoles.Count + NotAssignedRoles.Count + FailedRoles.Count;

    /// <summary>
    /// Indica si la operación fue completamente exitosa
    /// </summary>
    public bool IsFullySuccessful => FailedRoles.Count == 0;
}
