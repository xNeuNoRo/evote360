using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.WebApp.Filters;
using eVote360_Pro.Domain.Common;

namespace eVote360_Pro.WebApp.Controllers
{
    [SessionAuthorize]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(ICurrentUserService currentUserService, IDashboardService dashboardService) : base(currentUserService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            if (_currentUserService.Role == SystemRoles.Administrator)
            {
                var adminStats = await _dashboardService.GetGeneralStatisticsAsync();
                // Pass current year, or the dashboard service handles the latest
                var adminReport = await _dashboardService.GetAdminDashboardAsync(DateTime.Now.Year);
                ViewBag.ReportData = adminReport;
                
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
