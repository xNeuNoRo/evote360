using eVote360_Pro.Application.DTOs.Voting.Requests;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eVote360_Pro.WebApp.Controllers
{
    [AllowAnonymous]
    public class VotingController : Controller
    {
        private readonly IVotingService _votingService;
        private readonly IElectionService _electionService;
        private readonly ICitizenService _citizenService;

        public VotingController(
            IVotingService votingService,
            IElectionService electionService,
            ICitizenService citizenService
        )
        {
            _votingService = votingService;
            _electionService = electionService;
            _citizenService = citizenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
            {
                ViewBag.ErrorMessage = "No hay ningún proceso electoral activo en este momento.";
                return View("NoElection");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ValidateElectorRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var success = await _votingService.ValidateAndSendOtpAsync(request);
                if (success)
                {
                    var citizens = await _citizenService.GetAllAsync();
                    var citizen = citizens.FirstOrDefault(c =>
                        c.IdentityDocument == request.IdentityDocument
                    );

                    if (citizen != null)
                    {
                        TempData["VerifyCitizenId"] = citizen.Id.ToString();
                        TempData["IdentityDocument"] = request.IdentityDocument;
                        TempData["SuccessMessage"] =
                            "Validación exitosa. Hemos enviado un código de acceso a tu correo electrónico.";
                        return RedirectToAction(nameof(Verify));
                    }
                }

                ModelState.AddModelError(string.Empty, "Información inválida.");
                return View(request);
            }
            catch (DomainException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Verify()
        {
            var activeElection = await _electionService.GetActiveElectionAsync();
            if (activeElection == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var citizenIdStr = TempData.Peek("VerifyCitizenId")?.ToString();
            if (string.IsNullOrEmpty(citizenIdStr))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdentityDocument = TempData.Peek("IdentityDocument")?.ToString();

            var request = new VerifyCodeRequest(
                int.Parse(citizenIdStr),
                activeElection.Id,
                string.Empty
            );
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Verify(VerifyCodeRequest request)
        {
            TempData.Keep("VerifyCitizenId");
            TempData.Keep("IdentityDocument");

            try
            {
                var response = await _votingService.VerifyOtpAsync(request);

                if (!response.IsValid)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        response.ErrorMessage ?? "Código inválido."
                    );
                    return View(request);
                }

                HttpContext.Session.SetString("VerifiedCitizenId", request.CitizenId.ToString());
                HttpContext.Session.SetString("ElectionId", request.ElectionId.ToString());
                HttpContext.Session.SetString("VerificationCode", request.Code);

                TempData.Remove("VerifyCitizenId");
                TempData.Remove("IdentityDocument");

                return RedirectToAction(nameof(Ballot));
            }
            catch (DomainException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(request);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Ballot()
        {
            var citizenIdStr = HttpContext.Session.GetString("VerifiedCitizenId");
            var electionIdStr = HttpContext.Session.GetString("ElectionId");

            if (string.IsNullOrEmpty(citizenIdStr) || string.IsNullOrEmpty(electionIdStr))
            {
                return RedirectToAction(nameof(Index));
            }

            var electionId = Guid.Parse(electionIdStr);
            var ballot = await _votingService.GetBallotAsync(electionId);

            return View(ballot);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVote([FromBody] SubmitVoteRequest request)
        {
            var citizenIdStr = HttpContext.Session.GetString("VerifiedCitizenId");
            var verificationCode = HttpContext.Session.GetString("VerificationCode");

            if (string.IsNullOrEmpty(citizenIdStr) || string.IsNullOrEmpty(verificationCode))
            {
                return Unauthorized(new { message = "Sesión de votación expirada o inválida." });
            }

            request = request with
            {
                CitizenId = int.Parse(citizenIdStr),
                VerificationCode = verificationCode,
            };

            try
            {
                await _votingService.SubmitVoteAsync(request);

                HttpContext.Session.Remove("VerifiedCitizenId");
                HttpContext.Session.Remove("ElectionId");

                return Ok(new { success = true, redirectUrl = Url.Action(nameof(Success)) });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }
    }
}
