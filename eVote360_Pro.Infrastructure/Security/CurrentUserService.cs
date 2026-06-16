using System.Text.Json;
using eVote360_Pro.Application.DTOs.Auth.Responses;
using eVote360_Pro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Infrastructure.Security
{
    /// <summary>
    /// Implementación de infraestructura que extrae la identidad del usuario desde la sesión actual.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private AuthResponse? GetUserSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return null;

            var value = session.GetString("User");
            if (string.IsNullOrEmpty(value))
                return null;

            return JsonSerializer.Deserialize<AuthResponse>(value);
        }

        public Guid? UserId => GetUserSession()?.UserId;

        public int? PartyId => GetUserSession()?.PartyId;

        public bool IsAuthenticated => GetUserSession() != null;

        public string? Role => GetUserSession()?.RoleName;

        public string? FullName => GetUserSession()?.FullName;

        public string? Email => GetUserSession()?.Email;
    }
}
