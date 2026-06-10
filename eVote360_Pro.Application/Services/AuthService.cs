using eVote360_Pro.Application.DTOs.Auth.Requests;
using eVote360_Pro.Application.DTOs.Auth.Responses;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eVote360_Pro.Application.Services
{
    public class AuthService : IAuthService
    {
       private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // 1. Buscar el usuario con sus detalles de seguridad (Rol y Asignación Política)
            var user = await _userRepository.GetUserWithSecurityDetailsAsync(request.Username);

            if (user == null) 
            { 
                throw new DomainException("Credenciales incorrectas.", "Auth.InvalidCredentials");
            }

            // 2. Verificar la contraseña utilizando el hasher abstracto

            bool isPasswordValid = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid) 
            {
                throw new DomainException("Credenciales incorrectas.", "Auth.InvalidCredentials");
            }

            // 3. Validar si el usuario está activo antes de permitir el acceso
            if (!user.IsActive)
            {
                throw new DomainException("El usuario se encuentra inactivo.", "Auth.InactiveUser");
            }

            // 4. Generar el token JWT de acceso
            var token = _tokenService.GenerateToken(user);

            // 5. Retornar la respuesta con los datos requeridos por el contrato
            return new AuthResponse(
                UserId: user.Id,
                FullName: user.FirstName + " " + user.LastName,
                Email: user.Email,
                RoleName: user.Role?.Name ?? string.Empty,
                PartyId: user.LeaderAssignment?.PartyId,
                Token: token,
                Expiration: DateTime.UtcNow.AddHours(3) // Debe coincidir con la configuración de JwtSettings
            );
        }
    }
}
