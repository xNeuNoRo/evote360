using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(
            ICurrentUserService currentUserService,
            IDashboardService dashboardService
        )
            : base(currentUserService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index([FromQuery] int? year)
        {
            if (_currentUserService.Role == SystemRoles.Administrator)
            {
                var adminStats = await _dashboardService.GetGeneralStatisticsAsync();
                
                // Si no se provee año, intentamos usar el actual, o el más reciente
                int selectedYear = year ?? DateTime.Now.Year;
                
                var adminReportList = await _dashboardService.GetAdminDashboardAsync(selectedYear);
                ViewBag.ReportData = adminReportList;
                ViewBag.SelectedYear = selectedYear;

                return View("AdminDashboard", adminStats);
            }
            else if (_currentUserService.Role == SystemRoles.PoliticalLeader)
            {
                var leaderStats = await _dashboardService.GetLeaderStatisticsAsync();
                var leaderReport = await _dashboardService.GetLeaderDashboardAsync();
                ViewBag.ReportData = leaderReport;

                return View("LeaderDashboard", leaderStats);
            }

            return View("Index");
        }
    }
}
