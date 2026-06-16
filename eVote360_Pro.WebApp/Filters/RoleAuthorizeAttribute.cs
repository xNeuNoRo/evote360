using eVote360_Pro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360_Pro.WebApp.Filters
{
    /// <summary>
    /// Filtro de autorización que verifica si el usuario autenticado posee alguno de los roles permitidos.
    /// Redirige a Login si no hay sesión, o a AccessDenied si el rol no es válido.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentUserService =
                context.HttpContext.RequestServices.GetService(typeof(ICurrentUserService))
                as ICurrentUserService;

            // Primero verificamos si está autenticado
            if (currentUserService == null || !currentUserService.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Auth", action = "Index" })
                );
                return;
            }

            // Luego verificamos si tiene el rol correcto
            if (!_roles.Contains(currentUserService.Role))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Auth", action = "AccessDenied" })
                );
            }
        }
    }
}
