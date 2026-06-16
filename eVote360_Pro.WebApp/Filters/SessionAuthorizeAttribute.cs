using eVote360_Pro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eVote360_Pro.WebApp.Filters
{
    /// <summary>
    /// Filtro de autorización para validar que exista una sesión activa.
    /// Redirige al Login si el usuario no está autenticado.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var currentUserService =
                context.HttpContext.RequestServices.GetService(typeof(ICurrentUserService))
                as ICurrentUserService;

            if (currentUserService == null || !currentUserService.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Auth", action = "Index" })
                );
            }
        }
    }
}
