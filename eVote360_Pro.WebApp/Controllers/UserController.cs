using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests;
using eVote360_Pro.Application.DTOs.User.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.ViewModels.UserViewModels;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(SystemRoles.Administrator)]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IPoliticalPartyService _partyService;
        private readonly IPoliticalLeaderAssignmentService _assignmentService;

        public UserController(
            ICurrentUserService currentUserService,
            IUserService userService,
            IRoleService roleService,
            IPoliticalPartyService partyService,
            IPoliticalLeaderAssignmentService assignmentService
        )
            : base(currentUserService)
        {
            _userService = userService;
            _roleService = roleService;
            _partyService = partyService;
            _assignmentService = assignmentService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _userService.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Create()
        {
            await LoadRolesAsync();
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(
                    " | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );
                ShowAlert($"Validación falló: {errors}", "warning");
                await LoadRolesAsync();
                return View(model);
            }

            var request = new CreateUserRequest(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Username,
                model.Password,
                model.RoleId
            );
            await _userService.CreateAsync(request);

            ShowAlert("Usuario creado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var data = await _userService.GetByIdAsync(id);
            if (data == null)
                return NotFound();

            var vm = new UpdateUserViewModel
            {
                Id = data.Id,
                FirstName = data.FirstName,
                LastName = data.LastName,
                Email = data.Email,
                Username = data.Username,
                RoleId = data.RoleId,
                IsActive = data.IsActive,
            };

            await LoadRolesAsync();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(
                    " | ",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                );
                ShowAlert($"Validación falló: {errors}", "warning");
                await LoadRolesAsync();
                return View(model);
            }

            var request = new UpdateUserRequest(
                model.Id,
                model.FirstName,
                model.LastName,
                model.Email,
                model.Username,
                model.Password,
                model.RoleId,
                model.IsActive
            );
            await _userService.UpdateAsync(request);

            ShowAlert("Usuario actualizado exitosamente", "success");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(Guid id, bool activate)
        {
            await _userService.ToggleStatusAsync(id, activate);
            ShowAlert(
                activate ? "Usuario habilitado exitosamente" : "Usuario inhabilitado exitosamente",
                "success"
            );
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _roleService.GetRolesAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
        }
    }
}
