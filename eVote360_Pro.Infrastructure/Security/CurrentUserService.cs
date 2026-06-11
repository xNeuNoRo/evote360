using System.Security.Claims;
using eVote360_Pro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Infrastructure.Security
{
    /// <summary>
    /// Implementación de infraestructura que extrae la identidad del usuario desde el HttpContext actual.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var claim =
                    _httpContextAccessor
                        .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                        ?.Value
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

                return Guid.TryParse(claim, out var userId) ? userId : null;
            }
        }

        public int? PartyId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("party_id")?.Value;
                return int.TryParse(claim, out var partyId) ? partyId : null;
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
