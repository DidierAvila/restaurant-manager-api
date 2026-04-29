using RestaurantManager.Application.DTOs.AccessControl;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantManager.Application.Features.AccessControl.Commands.Users
{
    /// <summary>
    /// Comando para asignar múltiples roles a un usuario
    /// </summary>
    public class AssignMultipleRolesToUser
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Lista de IDs de roles a asignar
        /// </summary>
        public List<Guid> RoleIds { get; set; } = new();
    }

    /// <summary>
    /// Resultado de la asignación múltiple de roles
    /// </summary>
    public class MultipleRoleAssignmentResult
    {
        /// <summary>
        /// Roles asignados exitosamente
        /// </summary>
        public List<UserRoleDto> AssignedRoles { get; set; } = new();

        /// <summary>
        /// Roles que ya estaban asignados
        /// </summary>
        public List<UserRoleDto> ExistingRoles { get; set; } = new();

        /// <summary>
        /// Roles que no se pudieron asignar (errores)
        /// </summary>
        public List<string> FailedRoles { get; set; } = new();

        /// <summary>
        /// Número total de roles procesados
        /// </summary>
        public int TotalProcessed => AssignedRoles.Count + ExistingRoles.Count + FailedRoles.Count;

        /// <summary>
        /// Indica si la operación fue completamente exitosa
        /// </summary>
        public bool IsFullySuccessful => FailedRoles.Count == 0;
    }
}
