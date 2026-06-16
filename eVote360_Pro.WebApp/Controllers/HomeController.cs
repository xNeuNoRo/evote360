using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.ViewModels;
using eVote360_Pro.Application.Interfaces.Services;

namespace eVote360_Pro.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICurrentUserService _currentUserService;

    public HomeController(ILogger<HomeController> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public IActionResult Index()
    {
        // Si hay una sesión activa, lo mandamos al panel
        if (_currentUserService.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        // Si no está logueado, ve la Landing Page Pública (Portal del Votante)
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? message = null)
    {
        var errorMsg = message ?? TempData["ErrorMessage"]?.ToString() ?? "Ha ocurrido un error inesperado al procesar su solicitud.";
        var statusCode = HttpContext.Response.StatusCode == 200 ? 500 : HttpContext.Response.StatusCode;

        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            ErrorMessage = errorMsg,
            StatusCode = statusCode,
            ErrorTitle = "Error del Sistema"
        });
    }
}
