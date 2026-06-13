using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.WebApp.Filters;

namespace eVote360_Pro.WebApp.Controllers
{
    [RoleAuthorize(SystemRoles.PoliticalLeader)]
    public class CandidatePostAssignmentController : BaseController
    {
        private readonly ICandidatePostAssignmentService _assignmentService;
        private readonly IElectivePositionService _positionService;

        public CandidatePostAssignmentController(
            ICurrentUserService currentUserService,
            ICandidatePostAssignmentService assignmentService,
            IElectivePositionService positionService)
            : base(currentUserService)
        {
            _assignmentService = assignmentService;
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Armar Boletas";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Asignaciones", (string?)null, (string?)null)
            };

            var assignments = await _assignmentService.GetMyBallotAsync();
            int myPartyId = _currentUserService.PartyId ?? 0;
            ViewBag.MyPartyId = myPartyId;

            return View(assignments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Asignar Candidato";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Asignaciones", "Index", "CandidatePostAssignment"),
                ("Nueva Asignación", (string?)null, (string?)null)
            };

            var candidates = await _assignmentService.GetAvailableCandidatesAsync();
            var allPositions = await _positionService.GetAllAsync();
            var positions = allPositions.Where(p => p.IsActive).ToList();

            int myPartyId = _currentUserService.PartyId ?? 0;
            ViewBag.Candidates = candidates;
            ViewBag.Positions = positions;
            ViewBag.MyPartyId = myPartyId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int candidateId, int positionId, bool isAlly)
        {
            try
            {
                if (candidateId <= 0 || positionId <= 0)
                {
                    ShowAlert("Debe seleccionar un candidato y un puesto electivo.", "warning");
                    return RedirectToAction(nameof(Create));
                }

                // isAlly can be derived from frontend or safely overridden if backend does checking.
                // We trust the frontend checkbox or hidden field, but the service validates it anyway.

                var request = new SaveBallotAssignmentRequest(PositionId: positionId, CandidateId: candidateId, IsAlly: isAlly);
                await _assignmentService.SaveAssignmentAsync(request);

                ShowAlert("Candidato asignado a la boleta exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _assignmentService.RemoveAssignmentAsync(id);
                ShowAlert("Asignación eliminada exitosamente.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
