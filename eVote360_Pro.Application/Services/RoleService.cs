using eVote360_Pro.Application.DTOs.Role.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{

    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponse>> GetRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.ToResponse();
        }
    }
}
