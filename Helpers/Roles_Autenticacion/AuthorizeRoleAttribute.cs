using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CemSys3.Enumerables;

namespace CemSys3.Helpers.Roles_Autenticacion
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly RolUsuario[] _allowedRoles;

        public AuthorizeRoleAttribute(params RolUsuario[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;

            // Verificar autenticación
            var isAuthenticated = session.GetString("IsAuthenticated");
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            // Verificar roles
            var userRole = session.GetInt32("IdRol");
            if (userRole == null || !_allowedRoles.Contains((RolUsuario)userRole))
            {
                context.Result = new RedirectResult("~/");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
