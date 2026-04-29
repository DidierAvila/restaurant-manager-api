namespace RestaurantManager.Application.Core.Auth.Commands.RolePermissions
{
    /// <summary>
    /// Comando para remover múltiples permisos de un rol
    /// </summary>
    public class RemoveMultiplePermissionsFromRole
    {
        /// <summary>
        /// ID del rol
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Lista de IDs de permisos a remover
        /// </summary>
        public List<Guid> PermissionIds { get; set; } = new();
    }

    /// <summary>
    /// Resultado de la remoción múltiple de permisos
    /// </summary>
    public class MultiplePermissionRemovalResult
    {
        /// <summary>
        /// Permisos removidos exitosamente
        /// </summary>
        public List<Guid> RemovedPermissions { get; set; } = new();

        /// <summary>
        /// Permisos que no estaban asignados
        /// </summary>
        public List<Guid> NotAssignedPermissions { get; set; } = new();

        /// <summary>
        /// Permisos que no se pudieron remover (errores)
        /// </summary>
        public List<string> FailedPermissions { get; set; } = new();

        /// <summary>
        /// Número total de permisos procesados
        /// </summary>
        public int TotalProcessed => RemovedPermissions.Count + NotAssignedPermissions.Count + FailedPermissions.Count;

        /// <summary>
        /// Indica si la operación fue completamente exitosa
        /// </summary>
        public bool IsFullySuccessful => FailedPermissions.Count == 0;
    }
}
