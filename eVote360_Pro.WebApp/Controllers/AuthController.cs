using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.DTOs.Auth.Requests;
using eVote360_Pro.Application.ViewModels.AuthViewModels;

namespace eVote360_Pro.WebApp.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(ICurrentUserService currentUserService, IAuthService authService) 
            : base(currentUserService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Si el usuario ya está autenticado, no tiene sentido que vea el login
            if (_currentUserService.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Por favor, verifique los datos ingresados.", "warning");
                return View(model);
            }

            var request = new LoginRequest(model.Username, model.Password);
            
            // Si esto falla (credenciales incorrectas), el GlobalExceptionMiddleware lo atrapa
            // e inyecta el error en el TempData, devolviendo al usuario a esta misma vista
            // con un SweetAlert rojo hermoso.
            var authResponse = await _authService.LoginAsync(request);

            // Serializamos y guardamos la sesión pura
            HttpContext.Session.SetString("User", JsonSerializer.Serialize(authResponse));

            ShowAlert($"Bienvenido de vuelta, {authResponse.FullName}", "success");
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
