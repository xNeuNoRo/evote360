using Microsoft.AspNetCore.Mvc;
using eVote360_Pro.Application.DTOs.PoliticalAlliance.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.WebApp.Filters;

namespace eVote360_Pro.WebApp.Controllers
{
    [RoleAuthorize(SystemRoles.PoliticalLeader)]
    public class PoliticalAllianceController : BaseController
    {
        private readonly IPoliticalAllianceService _allianceService;

        public PoliticalAllianceController(ICurrentUserService currentUserService, IPoliticalAllianceService allianceService)
            : base(currentUserService)
        {
            _allianceService = allianceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Alianzas Políticas";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Alianzas", (string?)null, (string?)null)
            };

            var alliances = await _allianceService.GetMyAlliancesAsync();
            int myPartyId = _currentUserService.PartyId ?? 0;

            // Separate into the three required categories
            var pendingToRespond = alliances.Where(a => a.ReceiverPartyId == myPartyId && a.Status == AllianceStatus.Pending.ToString()).ToList();
            var sentRequests = alliances.Where(a => a.RequesterPartyId == myPartyId).ToList();
            var activeAlliances = alliances.Where(a => a.Status == AllianceStatus.Accepted.ToString()).ToList();

            ViewBag.PendingToRespond = pendingToRespond;
            ViewBag.SentRequests = sentRequests;
            ViewBag.ActiveAlliances = activeAlliances;
            ViewBag.MyPartyId = myPartyId;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Crear Solicitud de Alianza";
            ViewData["Breadcrumbs"] = new[]
            {
                ("Alianzas", "Index", "PoliticalAlliance"),
                ("Nueva Solicitud", (string?)null, (string?)null)
            };

            var availableParties = await _allianceService.GetAvailablePartiesForRequestAsync();
            return View(availableParties);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int receiverPartyId)
        {
            try
            {
                if (receiverPartyId <= 0)
                {
                    ShowAlert("Debe seleccionar un partido político.", "warning");
                    return RedirectToAction(nameof(Create));
                }

                var request = new CreateAllianceRequest(receiverPartyId);
                await _allianceService.RequestAllianceAsync(request);

                ShowAlert("Solicitud de alianza enviada exitosamente.");
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
                return RedirectToAction(nameof(Create));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            try
            {
                await _allianceService.AcceptAllianceAsync(id);
                ShowAlert("Alianza política aceptada formalmente.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                await _allianceService.RejectAllianceAsync(id);
                ShowAlert("Solicitud de alianza rechazada.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _allianceService.DeleteAllianceAsync(id);
                ShowAlert("Alianza política o solicitud eliminada exitosamente.");
            }
            catch (BusinessException ex)
            {
                ShowAlert(ex.Message, "error");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
