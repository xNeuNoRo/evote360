using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Enums;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace eVote360_Pro.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación concreta del repositorio de alianzas políticas.
    /// Gestiona las solicitudes y el historial de pactos entre partidos.
    /// </summary>
    public class PoliticalAllianceRepository
        : GenericRepository<PoliticalAlliance, int>,
            IPoliticalAlliancesRepository
    {
        public PoliticalAllianceRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> HasActiveAlliedAssignmentsAsync(int partyAId, int partyBId)
        {
            // Una alianza no puede romperse si existen candidatos de un partido postulados por el otro
            // Verificamos en ambas direcciones (A postula candidatos de B, o B postula candidatos de A)
            return await _context.CandidatePostAssignments.AnyAsync(a =>
                (a.PartyId == partyAId && a.Candidate.OriginalPartyId == partyBId)
                || (a.PartyId == partyBId && a.Candidate.OriginalPartyId == partyAId)
            );
        }

        public async Task<bool> AllianceOrRequestExistsAsync(int partyAId, int partyBId)
        {
            // Valida si ya existe un registro de alianza en cualquier estado (Pendiente, Aceptada, Rechazada)
            // Esto evita duplicidad de solicitudes entre los mismos partidos
            return await _dbSet.AnyAsync(a =>
                (a.RequesterPartyId == partyAId && a.ReceiverPartyId == partyBId)
                || (a.RequesterPartyId == partyBId && a.ReceiverPartyId == partyAId)
            );
        }
    }
}
