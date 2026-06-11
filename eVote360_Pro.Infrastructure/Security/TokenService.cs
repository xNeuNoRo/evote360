using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eVote360_Pro.Infrastructure.Security
{
    /// <summary>
    /// Implementación concreta del generador de tokens JWT.
    /// Crea los tokens de acceso que permiten la autenticación en el sistema.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDateTimeProvider _dateTimeProvider;

        public TokenService(IOptions<JwtSettings> jwtOptions, IDateTimeProvider dateTimeProvider)
        {
            _jwtSettings = jwtOptions.Value;
            _dateTimeProvider = dateTimeProvider;
        }

        public TokenResponse GenerateToken(User user)
        {
            // Creamos la security key a partir del secreto configurado
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            // Creamos las credenciales de firma usando HMAC SHA256
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Creamos los claims que se incluirán en el token. Estos claims representan la identidad y roles del usuario.
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // Agregamos el Rol del usuario como claim, si tiene uno asignado
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            // Agregamos el PartyId si es un Dirigente
            if (user.LeaderAssignment != null)
            {
                claims.Add(new Claim("party_id", user.LeaderAssignment.PartyId.ToString()));
            }

            var expiration = _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);

            // Creamos el descriptor del token, que incluye los claims,
            // la expiración, el emisor, la audiencia y las credenciales de firma
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Asignamos los claims al token
                Expires = expiration, // Establecemos la expiración del token
                Issuer = _jwtSettings.Issuer, // Establecemos el emisor del token
                Audience = _jwtSettings.Audience, // Establecemos la audiencia del token
                SigningCredentials = credentials, // Asignamos las credenciales de firma para asegurar la integridad del token
            };

            // Creamos el token usando el token handler de JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Finalmente, generamos el token y lo serializamos
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Devolvemos el resultado encapsulado
            return new TokenResponse(tokenHandler.WriteToken(token), expiration);
        }
    }
}
