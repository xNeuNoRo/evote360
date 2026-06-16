using eVote360_Pro.Application.DTOs.Candidate.Responses;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Requests;
using eVote360_Pro.Application.DTOs.CandidatePostAssignment.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de asignación de candidatos a puestos.
    /// </summary>
    public class CandidatePostAssignmentService : ICandidatePostAssignmentService
    {
        private readonly ICandidatePostAssignmentsRepository _assignmentRepository;
        private readonly ICandidatesRepository _candidateRepository;
        private readonly IElectivePositionsRepository _positionRepository;
        private readonly IPoliticalAlliancesRepository _allianceRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CandidatePostAssignmentService(
            ICandidatePostAssignmentsRepository assignmentRepository,
            ICandidatesRepository candidateRepository,
            IElectivePositionsRepository positionRepository,
            IPoliticalAlliancesRepository allianceRepository,
            IElectionRepository electionRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork
        )
        {
            _assignmentRepository = assignmentRepository;
            _candidateRepository = candidateRepository;
            _positionRepository = positionRepository;
            _allianceRepository = allianceRepository;
            _electionRepository = electionRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BallotAssignmentResponse>> GetMyBallotAsync()
        {
            int partyId = GetCurrentPartyId();

            var options = new QueryOptions<CandidatePostAssignment>
            {
                Filter = a => a.PartyId == partyId,
                Includes = new()
                {
                    a => a.Position,
                    a => a.Candidate,
                    a => a.Candidate.OriginalParty,
                },
                IsTracking = false,
            };

            var assignments = await _assignmentRepository.GetAllAsync(options);
            return assignments.ToResponse();
        }

        public async Task SaveAssignmentAsync(SaveBallotAssignmentRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                int myPartyId = GetCurrentPartyId();

                if (
                    await _assignmentRepository.IsPositionOccupiedInPartyAsync(
                        request.PositionId,
                        myPartyId
                    )
                )
                    throw new ValidationBusinessException(
                        nameof(request.PositionId),
                        "Puesto ocupado.",
                        "Assignment.PositionOccupied"
                    );

                if (
                    await _assignmentRepository.IsCandidateAssignedToAnyPostInPartyAsync(
                        request.CandidateId,
                        myPartyId
                    )
                )
                    throw new ValidationBusinessException(
                        nameof(request.CandidateId),
                        "Candidato asignado.",
                        "Assignment.CandidateAlreadyAssigned"
                    );

                var candidate =
                    await _candidateRepository.GetByIdAsync(request.CandidateId)
                    ?? throw new BusinessException(
                        "Candidato no encontrado.",
                        "Candidate.NotFound"
                    );
                    
                var position =
                    await _positionRepository.GetByIdAsync(request.PositionId)
                    ?? throw new BusinessException(
                        "Puesto no encontrado.",
                        "ElectivePosition.NotFound"
                    );

                if (request.IsAlly)
                {
                    if (candidate.OriginalPartyId == myPartyId)
                        throw new BusinessException(
                            "Propio no es aliado.",
                            "Assignment.InvalidAlly"
                        );

                    bool isAllied = await _allianceRepository.ExistsAsync(a =>
                        (
                            (
                                a.RequesterPartyId == myPartyId
                                && a.ReceiverPartyId == candidate.OriginalPartyId
                            )
                            || (
                                a.RequesterPartyId == candidate.OriginalPartyId
                                && a.ReceiverPartyId == myPartyId
                            )
                        )
                        && a.Status == AllianceStatus.Accepted
                    );

                    if (!isAllied)
                        throw new BusinessException("Sin pacto.", "Assignment.NoAlliance");

                    var original = await _assignmentRepository.GetOriginalAssignmentAsync(
                        request.CandidateId
                    );
                    if (original == null)
                        throw new BusinessException(
                            "Sin puesto origen.",
                            "Assignment.NoOriginalAssignment"
                        );
                    if (original.PositionId != request.PositionId)
                        throw new BusinessException(
                            "Puesto desigual.",
                            "Assignment.PositionMismatch"
                        );
                }
                else if (candidate.OriginalPartyId != myPartyId)
                {
                    throw new BusinessException(
                        "No es su candidato.",
                        "Assignment.NotYourCandidate"
                    );
                }

                var assignment = CandidatePostAssignment.Create(
                    request.CandidateId,
                    request.PositionId,
                    myPartyId,
                    request.IsAlly,
                    candidate.IsActive,
                    position.IsActive
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

        public async Task RemoveAssignmentAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var assignment =
                    await _assignmentRepository.GetByIdAsync(id)
                    ?? throw new BusinessException("No existe.", "Assignment.NotFound");

                if (assignment.PartyId != GetCurrentPartyId())
                    throw new BusinessException("Sin permiso.", "Assignment.AccessDenied");

                _assignmentRepository.Delete(assignment);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<CandidateResponse>> GetAvailableCandidatesAsync()
        {
            int myPartyId = GetCurrentPartyId();

            var alliances = await _allianceRepository.GetAllAsync(
                new QueryOptions<PoliticalAlliance>
                {
                    Filter = a =>
                        (a.RequesterPartyId == myPartyId || a.ReceiverPartyId == myPartyId)
                        && a.Status == AllianceStatus.Accepted,
                    IsTracking = false,
                }
            );

            var relevantIds = alliances
                .Select(a =>
                    a.RequesterPartyId == myPartyId ? a.ReceiverPartyId : a.RequesterPartyId
                )
                .Concat(new[] { myPartyId })
                .ToList();

            var candidates = await _candidateRepository.GetAllAsync(
                new QueryOptions<Candidate>
                {
                    Filter = c => relevantIds.Contains(c.OriginalPartyId) && c.IsActive,
                    Includes = new() { c => c.OriginalParty, c => c.Votes },
                    IsTracking = false,
                }
            );
            var myAssignments = await _assignmentRepository.GetAllAsync(
                new QueryOptions<CandidatePostAssignment>
                {
                    Filter = a => a.PartyId == myPartyId,
                    IsTracking = false,
                }
            );
            var assignedIds = myAssignments.Select(a => a.CandidateId).ToHashSet();
            var pool = candidates.Where(c => !assignedIds.Contains(c.Id)).ToList();

            var allyIds = pool.Where(c => c.OriginalPartyId != myPartyId)
                .Select(c => c.Id)
                .ToList();
            var originals = await _assignmentRepository.GetAllAsync(
                new QueryOptions<CandidatePostAssignment>
                {
                    Filter = a =>
                        allyIds.Contains(a.CandidateId) && a.PartyId == a.Candidate.OriginalPartyId,
                    IsTracking = false,
                }
            );
            var validAllyIds = originals.Select(a => a.CandidateId).ToHashSet();

            return pool.Where(c => c.OriginalPartyId == myPartyId || validAllyIds.Contains(c.Id))
                .ToResponse();
        }

        private async Task EnsureNoActiveElectionAsync()
        {
            if (await _electionRepository.AnyActiveElectionExistsAsync())
                throw new BusinessException(
                    "No se permiten cambios en la boleta mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
        }

        private int GetCurrentPartyId()
        {
            return _currentUserService.PartyId
                ?? throw new BusinessException(
                    "Sin afiliación política verificada.",
                    "Assignment.Unauthorized"
                );
        }
    }
}
