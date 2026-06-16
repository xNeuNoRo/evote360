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
    /// <summary>
    /// Implementación del servicio de asignación de mandos.
    /// </summary>
    public class PoliticalLeaderAssignmentService : IPoliticalLeaderAssignmentService
    {
        private readonly IPoliticalLeaderAssignmentsRepository _assignmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPoliticalPartiesRepository _partyRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PoliticalLeaderAssignmentService(
            IPoliticalLeaderAssignmentsRepository assignmentRepository,
            IUserRepository userRepository,
            IPoliticalPartiesRepository partyRepository,
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork
        )
        {
            _assignmentRepository = assignmentRepository;
            _userRepository = userRepository;
            _partyRepository = partyRepository;
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LeaderAssignmentResponse>> GetAllAsync()
        {
            var options = new QueryOptions<PoliticalLeaderAssignment>
            {
                Includes = new() { a => a.User, a => a.Party },
                IsTracking = false,
            };
            var assignments = await _assignmentRepository.GetAllAsync(options);
            return assignments.ToResponse();
        }

        public async Task CreateAssignmentAsync(SaveLeaderAssignmentRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var user =
                    await _userRepository.GetByIdAsync(request.UserId, u => u.Role!)
                    ?? throw new ValidationBusinessException(
                        nameof(request.UserId),
                        "Usuario no encontrado.",
                        "User.NotFound"
                    );

                if (await _assignmentRepository.IsUserAlreadyLeaderAsync(request.UserId))
                    throw new ValidationBusinessException(
                        nameof(request.UserId),
                        "Este usuario ya es dirigente.",
                        "Assignment.UserAlreadyHasParty"
                    );

                if (await _assignmentRepository.HasPartyAlreadyLeaderAsync(request.PartyId))
                    throw new ValidationBusinessException(
                        nameof(request.PartyId),
                        "Este partido ya tiene dirigente.",
                        "Assignment.PartyAlreadyHasLeader"
                    );

                var party =
                    await _partyRepository.GetByIdAsync(request.PartyId)
                    ?? throw new ValidationBusinessException(
                        nameof(request.PartyId),
                        "Este partido no encontrado.",
                        "PoliticalParty.NotFound"
                    );

                var assignment = PoliticalLeaderAssignment.Create(
                    request.UserId,
                    request.PartyId,
                    user.IsActive,
                    user.Role?.Name == SystemRoles.PoliticalLeader,
                    party.IsActive
                );
                await _assignmentRepository.AddAsync(assignment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveAssignmentAsync(Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var assignment =
                    await _assignmentRepository.GetByIdAsync(userId)
                    ?? throw new BusinessException(
                        "Asignación no encontrada.",
                        "Assignment.NotFound"
                    );

                _assignmentRepository.Delete(assignment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private async Task EnsureNoActiveElectionAsync()
        {
            if (await _electionRepository.AnyActiveElectionExistsAsync())
                throw new BusinessException(
                    "No se permiten gestionar dirigentes mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }
    }
}
