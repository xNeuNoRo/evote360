using eVote360_Pro.Application.DTOs.PoliticalParty.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.ViewModels.PoliticalPartyViewModels;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(SystemRoles.Administrator)]
    public class PoliticalPartyController : BaseController
    {
        private readonly IPoliticalPartyService _service;

        public PoliticalPartyController(
            ICurrentUserService currentUserService,
            IPoliticalPartyService service) : base(currentUserService)
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
            return View(new CreatePoliticalPartyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePoliticalPartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ShowAlert("Por favor, verifique los datos ingresados.", "warning");
                return View(model);
            }
            
            var request = new CreatePoliticalPartyRequest(model.Name, model.Acronym, model.Description, model.LogoFile);
            await _service.CreateAsync(request);
            
            ShowAlert("Partido político creado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null) return NotFound();

            var vm = new UpdatePoliticalPartyViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Acronym = data.Acronym,
                Description = data.Description,
                IsActive = data.IsActive,
                CurrentLogoUrl = data.LogoUrl
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePoliticalPartyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine("MODELSTATE ERRORS: " + errors);
                ShowAlert($"Validación falló: {errors}", "warning");
                return View(model);
            }

            var request = new UpdatePoliticalPartyRequest(model.Id, model.Name, model.Acronym, model.Description, model.LogoFile, model.IsActive);
            await _service.UpdateAsync(request);

            ShowAlert("Partido político actualizado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id, bool activate)
        {
            await _service.ToggleStatusAsync(id, activate);
            ShowAlert(activate ? "Partido activado exitosamente" : "Partido inactivado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }
    }
}
