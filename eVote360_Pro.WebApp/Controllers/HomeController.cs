using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.ViewModels;

namespace eVote360_Pro.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
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
