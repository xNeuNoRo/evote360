using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;

namespace eVote360_Pro.WebApp.Controllers
{
    [RoleAuthorize(SystemRoles.Administrator)]
    public class ResultController : BaseController
    {
        private readonly IResultService _resultService;
        private readonly IElectionService _electionService;

        public ResultController(
            ICurrentUserService currentUserService,
            IResultService resultService,
            IElectionService electionService)
            : base(currentUserService)
        {
            _resultService = resultService;
            _electionService = electionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Resultados Electorales";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Mantenimientos", (string?)null, (string?)null),
                ("Resultados", (string?)null, (string?)null)
            };

            var elections = await _electionService.GetAllAsync();
            var finishedElections = elections.Where(e => e.Status == eVote360_Pro.Domain.Enums.ElectionStatus.Finished.ToString()).ToList();
            var activeElection = elections.FirstOrDefault(e => e.Status == eVote360_Pro.Domain.Enums.ElectionStatus.Active.ToString());

            ViewBag.FinishedElections = finishedElections;
            ViewBag.ActiveElection = activeElection;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var election = await _electionService.GetByIdAsync(id);
            if (election == null) return NotFound();

            ViewData["Title"] = $"Resultados: {election.Name}";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Mantenimientos", (string?)null, (string?)null),
                ("Resultados", "Index", "Result"),
                (election.Name, (string?)null, (string?)null)
            };

            var report = await _resultService.GetReportAsync(id);
            return View(report);
        }
    }
}
