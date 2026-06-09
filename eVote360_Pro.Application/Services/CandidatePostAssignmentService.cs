using System.Security.Claims;
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
using Microsoft.AspNetCore.Http;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio que gestiona la configuración de la boleta electoral por partido.
    /// </summary>
    public class CandidatePostAssignmentService : ICandidatePostAssignmentService
    {
        private readonly ICandidatePostAssignmentsRepository _assignmentRepository;
        private readonly ICandidatesRepository _candidateRepository;
        private readonly IElectivePositionsRepository _positionRepository;
        private readonly IPoliticalAlliancesRepository _allianceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public CandidatePostAssignmentService(
            ICandidatePostAssignmentsRepository assignmentRepository,
            ICandidatesRepository candidateRepository,
            IElectivePositionsRepository positionRepository,
            IPoliticalAlliancesRepository allianceRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork
        )
        {
            _assignmentRepository = assignmentRepository;
            _candidateRepository = candidateRepository;
            _positionRepository = positionRepository;
            _allianceRepository = allianceRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BallotAssignmentResponse>> GetMyBallotAsync()
        {
            int partyId = GetCurrentPartyId();

            var options = new QueryOptions<CandidatePostAssignment>
            {
                Filter = a => a.PartyId == partyId,
                Includes = new() { a => a.Position, a => a.Candidate, a => a.Candidate.OriginalParty }
            };

            var assignments = await _assignmentRepository.GetAllAsync(options);
            return assignments.ToResponse();
        }

        public async Task SaveAssignmentAsync(SaveBallotAssignmentRequest request)
        {
            int myPartyId = GetCurrentPartyId();

            // 1. Validar que el puesto no esté ya ocupado en este partido
            if (await _assignmentRepository.IsPositionOccupiedInPartyAsync(request.PositionId, myPartyId))
            {
                throw new DomainException("Este puesto electivo ya está ocupado en su boleta.", "Assignment.PositionOccupied");
            }

            // 2. Validar que el candidato no aspire a otro puesto en este partido
            if (await _assignmentRepository.IsCandidateAssignedToAnyPostInPartyAsync(request.CandidateId, myPartyId))
            {
                throw new DomainException("Este candidato ya tiene una asignación en su boleta.", "Assignment.CandidateAlreadyAssigned");
            }

            var candidate = await _candidateRepository.GetByIdAsync(request.CandidateId)
                            ?? throw new DomainException("El candidato especificado no existe.", "Candidate.NotFound");

            var position = await _positionRepository.GetByIdAsync(request.PositionId)
                           ?? throw new DomainException("El puesto electivo especificado no existe.", "ElectivePosition.NotFound");

            // 3. Regla Crítica (Alianzas)
            if (request.IsAlly)
            {
                // El candidato debe pertenecer a otro partido
                if (candidate.OriginalPartyId == myPartyId)
                {
                    throw new DomainException("No puede asignar como aliado a un candidato de su propio partido.", "Assignment.InvalidAlly");
                }

                // Verificar pacto aceptado
                bool isAllied = await _allianceRepository.ExistsAsync(a => 
                    ((a.RequesterPartyId == myPartyId && a.ReceiverPartyId == candidate.OriginalPartyId) ||
                     (a.RequesterPartyId == candidate.OriginalPartyId && a.ReceiverPartyId == myPartyId)) &&
                    a.Status == AllianceStatus.Accepted);

                if (!isAllied)
                {
                    throw new DomainException("No puede asignar este candidato porque no existe un pacto aceptado con su partido de origen.", "Assignment.NoAlliance");
                }

                // Llamar a GetOriginalAssignmentAsync y validar que sea el mismo puesto
                var originalAssignment = await _assignmentRepository.GetOriginalAssignmentAsync(request.CandidateId);
                if (originalAssignment == null)
                {
                    throw new DomainException("El candidato aliado aún no tiene un puesto asignado en su partido de origen.", "Assignment.NoOriginalAssignment");
                }

                if (originalAssignment.PositionId != request.PositionId)
                {
                    throw new DomainException("El candidato aliado debe aspirar al mismo puesto que tiene en su partido de origen.", "Assignment.PositionMismatch");
                }
            }
            else
            {
                if (candidate.OriginalPartyId != myPartyId)
                {
                    throw new DomainException("El candidato no pertenece a su partido. Debe marcarlo como aliado si existe un pacto.", "Assignment.NotYourCandidate");
                }
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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAssignmentAsync(int id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id)
                             ?? throw new DomainException("La asignación no existe.", "Assignment.NotFound");

            if (assignment.PartyId != GetCurrentPartyId())
            {
                throw new DomainException("No tiene permisos para remover esta asignación.", "Assignment.AccessDenied");
            }

            _assignmentRepository.Delete(assignment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CandidateResponse>> GetAvailableCandidatesAsync()
        {
            int myPartyId = GetCurrentPartyId();

            // Obtener IDs de partidos aliados (pactos aceptados)
            var alliances = await _allianceRepository.GetAllAsync(new QueryOptions<PoliticalAlliance>
            {
                Filter = a => (a.RequesterPartyId == myPartyId || a.ReceiverPartyId == myPartyId) && a.Status == AllianceStatus.Accepted
            });

            var alliedPartyIds = alliances
                .Select(a => a.RequesterPartyId == myPartyId ? a.ReceiverPartyId : a.RequesterPartyId)
                .ToList();

            var relevantPartyIds = new List<int>(alliedPartyIds) { myPartyId };

            // Obtener candidatos activos de estos partidos
            var candidates = await _candidateRepository.GetAllAsync(new QueryOptions<Candidate>
            {
                Filter = c => relevantPartyIds.Contains(c.OriginalPartyId) && c.IsActive,
                Includes = new() { c => c.OriginalParty, c => c.Votes }
            });

            // Filtrar los que ya están asignados en MI boleta
            var myAssignments = await _assignmentRepository.GetAllAsync(new QueryOptions<CandidatePostAssignment>
            {
                Filter = a => a.PartyId == myPartyId
            });

            var assignedCandidateIds = myAssignments.Select(a => a.CandidateId).ToHashSet();

            var availableCandidates = candidates.Where(c => !assignedCandidateIds.Contains(c.Id)).ToList();

            // Para candidatos aliados, solo incluirlos si ya tienen su puesto original asignado
            var finalCandidates = new List<Candidate>();
            foreach(var c in availableCandidates)
            {
                if (c.OriginalPartyId != myPartyId)
                {
                    var original = await _assignmentRepository.GetOriginalAssignmentAsync(c.Id);
                    if (original != null) finalCandidates.Add(c);
                }
                else
                {
                    finalCandidates.Add(c);
                }
            }

            return finalCandidates.ToResponse();
        }

        private int GetCurrentPartyId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var partyIdClaim = user?.FindFirst("party_id")?.Value;

            if (string.IsNullOrEmpty(partyIdClaim) || !int.TryParse(partyIdClaim, out int partyId))
            {
                throw new DomainException("No se pudo identificar el partido político del usuario logueado.", "Assignment.Unauthorized");
            }

            return partyId;
        }
    }
}
