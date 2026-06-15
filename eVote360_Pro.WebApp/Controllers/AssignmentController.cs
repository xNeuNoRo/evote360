using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    [RoleAuthorize(SystemRoles.Administrator)]
    public class AssignmentController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IPoliticalPartyService _partyService;
        private readonly IPoliticalLeaderAssignmentService _assignmentService;

        public AssignmentController(
            ICurrentUserService currentUserService,
            IUserService userService,
            IPoliticalPartyService partyService,
            IPoliticalLeaderAssignmentService assignmentService
        ) : base(currentUserService)
        {
            _userService = userService;
            _partyService = partyService;
            _assignmentService = assignmentService;
        }

        [HttpGet]
        public async Task<IActionResult> AssignLeader(Guid? userId)
        {
            await LoadSelectDataAsync(userId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignLeader(Guid userId, int partyId)
        {
            if (userId == Guid.Empty || partyId <= 0)
            {
                ShowAlert("Debe seleccionar un usuario y un partido político", "warning");
                await LoadSelectDataAsync();
                return View();
            }

            try
            {
                var request = new SaveLeaderAssignmentRequest(userId, partyId);
                await _assignmentService.CreateAssignmentAsync(request);
                ShowAlert("Partido asignado exitosamente al dirigente", "success");
                return RedirectToAction("Index", "User");
            }
            catch (Exception ex)
            {
                ShowAlert($"Error: {ex.Message}", "error");
                await LoadSelectDataAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLeader(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                ShowAlert("Debe seleccionar un dirigente", "warning");
                return RedirectToAction(nameof(AssignLeader));
            }

            try
            {
                await _assignmentService.RemoveAssignmentAsync(userId);
                ShowAlert("Se ha removido el partido asignado al dirigente", "success");
                return RedirectToAction(nameof(AssignLeader));
            }
            catch (Exception ex)
            {
                ShowAlert($"Error: {ex.Message}", "error");
                return RedirectToAction(nameof(AssignLeader));
            }
        }

        private async Task LoadSelectDataAsync(Guid? selectedUserId = null)
        {
            var users = await _userService.GetAllAsync();
            var activeLeaders = users.Where(u => u.RoleName == SystemRoles.PoliticalLeader && u.IsActive).ToList();
            
            var parties = await _partyService.GetAllAsync();
            var activeParties = parties.Where(p => p.IsActive).ToList();

            ViewBag.Users = new SelectList(activeLeaders, "Id", "FullName", selectedUserId);
            ViewBag.Parties = new SelectList(activeParties, "Id", "Name");
            
            var assignedLeaders = activeLeaders.Where(u => u.AssignedPartyId.HasValue).ToList();
            ViewBag.AssignedLeaders = new SelectList(assignedLeaders, "Id", "FullName", selectedUserId);

            if (selectedUserId.HasValue)
            {
                var selectedUser = activeLeaders.FirstOrDefault(u => u.Id == selectedUserId.Value);
                if (selectedUser != null)
                {
                    ViewBag.SelectedUserFullName = selectedUser.FullName;
                    ViewBag.HasPartyAssigned = selectedUser.AssignedPartyId.HasValue;
                }
            }
        }
    }
}
