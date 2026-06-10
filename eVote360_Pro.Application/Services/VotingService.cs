using eVote360_Pro.Application.DTOs.Voting.Requests;
using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;
using eVote360_Pro.Shared.Interfaces.Messaging;
using eVote360_Pro.Shared.Interfaces.OCR;

namespace eVote360_Pro.Application.Services
{
    public class VotingService : IVotingService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICitizenRepository _citizenRepository;
        private readonly IVoterParticipationRepository _voterParticipationRepository;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly ICandidatePostAssignmentsRepository _candidatePostAssignmentsRepository;
        private readonly IElectivePositionsRepository _electivePositionsRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IOcrService _ocrService;
        private readonly IEmailService _emailService;
        private readonly IVerificationCodeGenerator _verificationCodeGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public VotingService(
            IElectionRepository electionRepository,
            ICitizenRepository citizenRepository,
            IVoterParticipationRepository voterParticipationRepository,
            IVerificationCodeRepository verificationCodeRepository,
            ICandidatePostAssignmentsRepository candidatePostAssignmentsRepository,
            IElectivePositionsRepository electivePositionsRepository,
            IVoteRepository voteRepository,
            IOcrService ocrService,
            IEmailService emailService,
            IVerificationCodeGenerator verificationCodeGenerator,
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider
        )
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _voterParticipationRepository = voterParticipationRepository;
            _verificationCodeRepository = verificationCodeRepository;
            _candidatePostAssignmentsRepository = candidatePostAssignmentsRepository;
            _electivePositionsRepository = electivePositionsRepository;
            _voteRepository = voteRepository;
            _ocrService = ocrService;
            _emailService = emailService;
            _verificationCodeGenerator = verificationCodeGenerator;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> ValidateAndSendOtpAsync(ValidateElectorRequest request)
        {
            var election = await _electionRepository.GetActiveElectionAsync();
            if (election == null)
                throw new DomainException(
                    "No existe una elección activa.",
                    "Voting.NoActiveElection"
                );

            var citizen = await _citizenRepository.GetByIdentityDocumentAsync(
                request.IdentityDocument.Trim()
            );

            if (citizen == null)
                throw new DomainException(
                    "Ciudadano no encontrado.",
                    "Voting.CitizenNotFound"
                );

            if (!citizen.IsActive)
                throw new DomainException(
                    "El ciudadano se encuentra inactivo.",
                    "Voting.CitizenInactive"
                );

            if (await _voterParticipationRepository.HasAlreadyVotedAsync(
                    citizen.Id,
                    election.Id
                ))
            {
                throw new DomainException(
                    "Este ciudadano ya emitió su voto en la elección activa.",
                    "Voting.AlreadyVoted"
                );
            }

            var ocrResult = await _ocrService.ProcessIdentityCardAsync(request.IdCardImage);
            if (!ocrResult.IsSuccess || !ocrResult.IsDocumentValid)
                throw new DomainException(
                    "No fue posible validar el documento de identidad.",
                    "Voting.OcrInvalid"
                );

            if (!string.Equals(
                    request.IdentityDocument.Trim(),
                    ocrResult.IdentityNumber?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                throw new DomainException(
                    "El número de cédula extraído por OCR no coincide.",
                    "Voting.OcrMismatch"
                );
            }

            await _verificationCodeRepository.InvalidatePreviousCodesAsync(
                citizen.Id,
                election.Id
            );

            var otpCode = _verificationCodeGenerator.Generate(6);
            var verificationCode = VerificationCode.Create(
                citizen.Id,
                election.Id,
                otpCode,
                _dateTimeProvider.UtcNow
            );

            await _verificationCodeRepository.AddAsync(verificationCode);
            await _unitOfWork.SaveChangesAsync();

            var emailModel = new OtpEmailModel(
                $"{citizen.FirstName} {citizen.LastName}",
                otpCode
            );

            var emailSent = await _emailService.SendEmailAsync(
                citizen.Email,
                "Código OTP para votación.",
                "OtpVerification",
                emailModel
            );

            if (!emailSent)
                throw new DomainException(
                    "No se pudo enviar el código OTP por correo electrónico.",
                    "Voting.EmailFailed"
                );

            return true;
        }

        public async Task<VerifyCodeResponse> VerifyOtpAsync(VerifyCodeRequest request)
        {
            var code = await _verificationCodeRepository.GetValidCodeAsync(
                request.CitizenId,
                request.ElectionId,
                request.Code
            );

            if (code == null)
                return new VerifyCodeResponse(false, "Código inválido o expirado.");

            try
            {
                code.Use(_dateTimeProvider.UtcNow);
                await _unitOfWork.SaveChangesAsync();
                return new VerifyCodeResponse(true, null);
            }
            catch (DomainException exception)
            {
                return new VerifyCodeResponse(false, exception.Message);
            }
        }

        public async Task<VoterBallotResponse> GetBallotAsync(Guid electionId)
        {
            var election = await _electionRepository.GetByIdAsync(electionId);
            if (election == null || !election.IsActive)
                throw new DomainException(
                    "Elección inválida o no activa.",
                    "Voting.InvalidElection"
                );

            var positions = await _electivePositionsRepository.GetAllAsync(
                new Domain.Common.QueryOptions<ElectivePosition>
                {
                    Filter = position => position.IsActive,
                }
            );

            var assignments = await _candidatePostAssignmentsRepository.GetAllAsync(
                new Domain.Common.QueryOptions<CandidatePostAssignment>
                {
                    Filter = assignment =>
                        assignment.Position.IsActive
                        && assignment.Candidate.IsActive
                        && assignment.Party.IsActive,
                    Includes =
                    {
                        assignment => assignment.Candidate,
                        assignment => assignment.Party,
                        assignment => assignment.Position,
                    },
                }
            );

            var ballotPositions = positions.Select(position =>
            {
                var candidates = assignments
                    .Where(a => a.PositionId == position.Id)
                    .Select(a => new BallotCandidateResponse(
                        a.CandidateId,
                        $"{a.Candidate.FirstName} {a.Candidate.LastName}",
                        a.PartyId,
                        a.Party.Name,
                        a.Party.Acronym,
                        a.Candidate.PhotoPath,
                        a.Party.LogoPath
                    ))
                    .ToList();

                return position.ToBallotPosition(candidates);
            }).ToList();

            return election.ToVoterBallot(ballotPositions);
        }

        public async Task SubmitVoteAsync(SubmitVoteRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var election = await _electionRepository.GetByIdAsync(request.ElectionId);
                if (election == null || !election.IsActive)
                    throw new DomainException(
                        "Elección inválida o no activa.",
                        "Voting.InvalidElection"
                    );

                var citizen = await _citizenRepository.GetByIdAsync(request.CitizenId);
                if (citizen == null || !citizen.IsActive)
                    throw new DomainException(
                        "Ciudadano inválido o inactivo.",
                        "Voting.InvalidCitizen"
                    );

                if (await _voterParticipationRepository.HasAlreadyVotedAsync(
                        request.CitizenId,
                        request.ElectionId
                    ))
                {
                    throw new DomainException(
                        "Este ciudadano ya ha registrado su participación.",
                        "Voting.AlreadyVoted"
                    );
                }

                var usedCode = await _verificationCodeRepository.GetFirstOrDefaultAsync(
                    new Domain.Common.QueryOptions<VerificationCode>
                    {
                        Filter = code =>
                            code.CitizenId == request.CitizenId
                            && code.ElectionId == request.ElectionId
                            && code.Code == request.VerificationCode
                            && code.IsUsed,
                        IsTracking = true,
                    }
                );

                if (usedCode == null)
                    throw new DomainException(
                        "El código OTP debe haberse utilizado antes de enviar el voto.",
                        "Voting.CodeNotUsed"
                    );

                var participation = VoterParticipation.Create(
                    request.CitizenId,
                    request.ElectionId,
                    citizen.IsActive,
                    election.IsActive
                );

                await _voterParticipationRepository.AddAsync(participation);

                var votes = request.Selections
                    .Select(selection =>
                        Vote.Create(
                            request.ElectionId,
                            selection.PositionId,
                            selection.CandidateId,
                            selection.PartyId
                        )
                    )
                    .ToList();

                await _voteRepository.AddRangeAsync(votes);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        private sealed record OtpEmailModel(string FullName, string Code) : IEmailModel;
    }
}
