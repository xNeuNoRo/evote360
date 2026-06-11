using eVote360_Pro.Application.DTOs.Auth.Requests;
using eVote360_Pro.Application.DTOs.Auth.Responses;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de autenticación.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserWithSecurityDetailsAsync(request.Username);
            if (user == null)
            {
                throw new BusinessException(
                    "Nombre de usuario no encontrado.",
                    "Auth.InvalidCredentials"
                );
            }

            // Verificamos la pass
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new BusinessException("Contraseña incorrecta.", "Auth.InvalidCredentials");
            }

            // Si el usuario no está activo, no puede iniciar sesión
            if (!user.IsActive)
            {
                throw new BusinessException(
                    "El usuario se encuentra inactivo.",
                    "General.Unexpected"
                );
            }

            // Si el usuario es un líder político, verificamos que su partido esté activo
            if (
                user.Role?.Name == SystemRoles.PoliticalLeader
                && (user.LeaderAssignment == null || !user.LeaderAssignment.Party.IsActive)
            )
            {
                throw new BusinessException(
                    "Acceso denegado: Partido político inactivo.",
                    "Auth.PartyInactive"
                );
            }

            // Generamos un token JWT
            var tokenResult = _tokenService.GenerateToken(user);

            return new AuthResponse(
                UserId: user.Id,
                FullName: $"{user.FirstName} {user.LastName}",
                Email: user.Email,
                RoleName: user.Role?.Name ?? "Sin Rol",
                PartyId: user.LeaderAssignment?.PartyId,
                Token: tokenResult.Token,
                Expiration: tokenResult.Expiration
            );
        }
    }
}
