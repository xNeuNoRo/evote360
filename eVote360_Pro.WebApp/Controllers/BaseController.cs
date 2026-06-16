using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.Interfaces.Services;

namespace eVote360_Pro.WebApp.Controllers
{
    /// <summary>
    /// Controlador base del cual deben heredar todos los controladores que requieran autenticación.
    /// Centraliza utilidades repetitivas como inyección de alertas y acceso a la sesión.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly ICurrentUserService _currentUserService;

        protected BaseController(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Muestra una notificación SweetAlert en la siguiente vista cargada.
        /// </summary>
        /// <param name="message">El mensaje a mostrar</param>
        /// <param name="type">El tipo de alerta (success, error, warning, info)</param>
        protected void ShowAlert(string message, string type = "success")
        {
            TempData["SweetAlertType"] = type;
            TempData["SweetAlertMessage"] = message;
        }
    }
}
