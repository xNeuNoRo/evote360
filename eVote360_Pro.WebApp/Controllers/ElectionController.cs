using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.DTOs.Election.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.WebApp.Filters;

namespace eVote360_Pro.WebApp.Controllers
{
    [RoleAuthorize(SystemRoles.Administrator)]
    public class ElectionController : BaseController
    {
        private readonly IElectionService _electionService;

        public ElectionController(
            ICurrentUserService currentUserService,
            IElectionService electionService)
            : base(currentUserService)
        {
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Elecciones";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Mantenimientos", (string?)null, (string?)null),
                ("Elecciones", (string?)null, (string?)null)
            };

            var elections = await _electionService.GetAllAsync();
            return View(elections);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Programar Elección";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Mantenimientos", (string?)null, (string?)null),
                ("Elecciones", "Index", "Election"),
                ("Nueva Elección", (string?)null, (string?)null)
            };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateElectionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                await _electionService.CreateAsync(request);
                ShowAlert("Proceso electoral programado exitosamente. Se encuentra en estado 'Pendiente'.");
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
                return View(request);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await _electionService.ActivateAsync(id);
                ShowAlert("Elección activada exitosamente. Todas las operaciones políticas han sido bloqueadas y el sistema de votación está abierto.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Finish(Guid id)
        {
            try
            {
                await _electionService.FinishAsync(id);
                ShowAlert("La elección ha finalizado de forma definitiva. Los resultados están ahora disponibles para el escrutinio.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
