using eVote360_Pro.Application.DTOs.Citizen.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.ViewModels.CitizenViewModels;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(SystemRoles.Administrator)]
    public class CitizenController : BaseController
    {
        private readonly ICitizenService _service;

        public CitizenController(
            ICurrentUserService currentUserService,
            ICitizenService service) : base(currentUserService)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAllAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View(new CreateCitizenViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCitizenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ShowAlert($"Validación falló: {errors}", "warning");
                return View(model);
            }
            
            var request = new CreateCitizenRequest(model.IdentityDocument, model.FirstName, model.LastName, model.Email);
            await _service.CreateAsync(request);
            
            ShowAlert("Ciudadano registrado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            var vm = new UpdateCitizenViewModel
            {
                Id = data.Id,
                IdentityDocument = data.IdentityDocument,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                IsActive = data.IsActive
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCitizenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ShowAlert($"Validación falló: {errors}", "warning");
                return View(model);
            }

            var request = new UpdateCitizenRequest(model.Id, model.IdentityDocument, model.FirstName, model.LastName, model.Email, model.IsActive);
            await _service.UpdateAsync(request);

            ShowAlert("Ciudadano actualizado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool activate)
        {
            await _service.ToggleStatusAsync(id, activate);
            ShowAlert(activate ? "Ciudadano habilitado exitosamente" : "Ciudadano inhabilitado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }
    }
}
