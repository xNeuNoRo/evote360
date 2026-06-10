using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Requests;
using eVote360_Pro.Application.DTOs.PoliticalLeaderAssignment.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    public class PoliticalLeaderAssignmentService : IPoliticalLeaderAssignmentService
    {
        private readonly IPoliticalLeaderAssignmentsRepository _assignmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PoliticalLeaderAssignmentService(
            IPoliticalLeaderAssignmentsRepository assignmentRepository,
            IUserRepository userRepository,
            IPoliticalPartiesRepository partyRepository,
            IUnitOfWork unitOfWork
        )
        {
            _assignmentRepository = assignmentRepository;
            _userRepository = userRepository;
            _partyRepository = partyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LeaderAssignmentResponse>> GetAllAsync()
        {
            var assignments = await _assignmentRepository.GetAllAsync(new QueryOptions<PoliticalLeaderAssignment>
            {
                Includes = new() { a => a.User, a => a.Party }
            });

            return assignments.ToResponse();
        }

        public async Task CreateAssignmentAsync(SaveLeaderAssignmentRequest request)
        {
            // 1. Validar que el usuario exista y tenga rol "Dirigente"
            var user = await _userRepository.GetByIdAsync(request.UserId, u => u.Role!)
                       ?? throw new DomainException("El usuario especificado no existe.", "User.NotFound");

            bool isDirigente = user.Role?.Name == SystemRoles.PoliticalLeader;

            // 2. Validar que el usuario no tenga ya un partido
            if (await _assignmentRepository.IsUserAlreadyLeaderAsync(request.UserId))
            {
                throw new DomainException("El usuario ya tiene un partido político asignado.", "Assignment.UserAlreadyHasParty");
            }

            // 3. Validar que el partido no tenga ya un dirigente
            if (await _assignmentRepository.HasPartyAlreadyLeaderAsync(request.PartyId))
            {
                throw new DomainException("El partido político ya tiene un dirigente asignado.", "Assignment.PartyAlreadyHasLeader");
            }

            var party = await _partyRepository.GetByIdAsync(request.PartyId)
                        ?? throw new DomainException("El partido político especificado no existe.", "PoliticalParty.NotFound");

            // Crear la asignación utilizando el factory method del dominio que encapsula el resto de validaciones
            var assignment = PoliticalLeaderAssignment.Create(
                request.UserId,
                request.PartyId,
                user.IsActive,
                isDirigente,
                party.IsActive
            );

            await _assignmentRepository.AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAssignmentAsync(Guid userId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(userId)
                             ?? throw new DomainException("La asignación no existe.", "Assignment.NotFound");

            _assignmentRepository.Delete(assignment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
