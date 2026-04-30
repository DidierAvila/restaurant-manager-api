using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestaurantManager.Api.Attributes
{
    /// <summary>
    /// Atributo personalizado para validar permisos específicos en controladores y acciones.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;

        /// <summary>
        /// Constructor del atributo RequirePermission.
        /// </summary>
        /// <param name="permission">El permiso requerido para acceder al recurso.</param>
        public RequirePermissionAttribute(string permission)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
        }

        /// <summary>
        /// Método que se ejecuta para validar la autorización.
        /// </summary>
        /// <param name="context">Contexto de autorización.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Verificar si el usuario está autenticado
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Usuario no autenticado"
                });
                return;
            }

            // Verificar si el usuario tiene el permiso requerido en los claims del JWT
            var permissionClaims = context.HttpContext.User.FindAll("permission").Select(c => c.Value);

            if (!permissionClaims.Contains(_permission))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
