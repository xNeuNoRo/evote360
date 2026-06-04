using eVote360_Pro.Domain.Entities;

namespace eVote360_Pro.Domain.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
