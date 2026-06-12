using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.DTOs.Candidate.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.ViewModels.CandidateViewModels;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.WebApp.Filters;

namespace eVote360_Pro.WebApp.Controllers
{
    [RoleAuthorize(SystemRoles.PoliticalLeader)]
    public class CandidateController : BaseController
    {
        private readonly ICandidateService _candidateService;

        public CandidateController(ICurrentUserService currentUserService, ICandidateService candidateService)
            : base(currentUserService)
        {
            _candidateService = candidateService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Mis Candidatos";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Candidatos", (string?)null, (string?)null)
            };
            var candidates = await _candidateService.GetAllAsync();
            return View(candidates);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Nuevo Candidato";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Candidatos", "Index", "Candidate"),
                ("Crear", (string?)null, (string?)null)
            };
            return View(new CreateCandidateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCandidateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Verifique los datos ingresados.", "warning");
                return View(model);
            }

            try
            {
                var request = new CreateCandidateRequest
                (
                    FirstName: model.FirstName,
                    LastName: model.LastName,
                    PhotoFile: model.PhotoFile!,
                    OriginalPartyId: null // El servicio lo auto-asigna
                );

                await _candidateService.CreateAsync(request);
                ShowAlert("Candidato registrado exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                ShowAlert(ex.Message, "error");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var candidate = await _candidateService.GetByIdAsync(id);
            if (candidate == null) return NotFound();

            ViewData["Title"] = "Editar Candidato";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Candidatos", "Index", "Candidate"),
                ("Editar", (string?)null, (string?)null)
            };

            var model = new UpdateCandidateViewModel
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                LastName = candidate.LastName,
                IsActive = candidate.IsActive,
                CurrentPhotoPath = candidate.PhotoPath,
                HasParticipated = candidate.VotesCount > 0 // This is a heuristic, the service handles real check
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCandidateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Verifique los datos ingresados.", "warning");
                return View(model);
            }

            try
            {
                var request = new UpdateCandidateRequest
                (
                    Id: model.Id,
                    FirstName: model.FirstName,
                    LastName: model.LastName,
                    PhotoFile: model.PhotoFile,
                    IsActive: model.IsActive
                );

                await _candidateService.UpdateAsync(request);
                ShowAlert("Candidato actualizado exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (ValidationBusinessException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                ShowAlert(ex.Message, "error");
                return View(model);
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool activate)
        {
            try
            {
                await _candidateService.ToggleStatusAsync(id, activate);
                ShowAlert(activate ? "Candidato activado exitosamente." : "Candidato inactivado exitosamente.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
