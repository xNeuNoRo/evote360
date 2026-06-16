using eVote360_Pro.Application.DTOs.ElectivePosition.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.ViewModels.ElectivePositionViewModels;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(SystemRoles.Administrator)]
    public class ElectivePositionController : BaseController
    {
        private readonly IElectivePositionService _service;

        public ElectivePositionController(
            ICurrentUserService currentUserService,
            IElectivePositionService service) : base(currentUserService)
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
            return View(new CreateElectivePositionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateElectivePositionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Por favor, verifique los datos ingresados.", "warning");
                return View(model);
            }
            
            var request = new CreateElectivePositionRequest(model.Name, model.Description);
            await _service.CreateAsync(request);
            
            ShowAlert("Puesto creado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            var vm = new UpdateElectivePositionViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                IsActive = data.IsActive
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateElectivePositionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Por favor, verifique los datos ingresados.", "warning");
                return View(model);
            }

            var request = new UpdateElectivePositionRequest(model.Id, model.Name, model.Description, model.IsActive);
            await _service.UpdateAsync(request);

            ShowAlert("Puesto actualizado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool activate)
        {
            await _service.ToggleStatusAsync(id, activate);
            ShowAlert(activate ? "Puesto activado exitosamente" : "Puesto inactivado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }
    }
}
