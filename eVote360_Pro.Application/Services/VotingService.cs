using eVote360_Pro.Application.DTOs.Voting.Requests;
using eVote360_Pro.Application.DTOs.Voting.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.Models.Emails;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Providers;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;
using eVote360_Pro.Domain.ValueObjects;
using eVote360_Pro.Shared.Interfaces.Messaging;
using eVote360_Pro.Shared.Interfaces.OCR;
using eVote360_Pro.Shared.Interfaces.Storage;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de votación.
    /// </summary>
    public class VotingService : IVotingService
    {
        private readonly IElectionRepository _electionRepository;
        private readonly ICitizenRepository _citizenRepository;
        private readonly IVoterParticipationRepository _voterParticipationRepository;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly ICandidatePostAssignmentsRepository _assignmentRepository;
        private readonly IElectivePositionsRepository _positionRepository;
        private readonly IVoteRepository _voteRepository;
        private readonly IOcrService _ocrService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IVerificationCodeGenerator _codeGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDateTimeProvider _dateTimeProvider;

        public VotingService(
            IElectionRepository electionRepository,
            ICitizenRepository citizenRepository,
            IVoterParticipationRepository voterParticipationRepository,
            IVerificationCodeRepository verificationCodeRepository,
            ICandidatePostAssignmentsRepository assignmentRepository,
            IElectivePositionsRepository positionRepository,
            IVoteRepository voteRepository,
            IOcrService ocrService,
            IEmailService emailService,
            IFileService fileService,
            IVerificationCodeGenerator codeGenerator,
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider
        )
        {
            _electionRepository = electionRepository;
            _citizenRepository = citizenRepository;
            _voterParticipationRepository = voterParticipationRepository;
            _verificationCodeRepository = verificationCodeRepository;
            _assignmentRepository = assignmentRepository;
            _positionRepository = positionRepository;
            _voteRepository = voteRepository;
            _ocrService = ocrService;
            _emailService = emailService;
            _fileService = fileService;
            _codeGenerator = codeGenerator;
            _unitOfWork = unitOfWork;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<bool> ValidateAndSendOtpAsync(ValidateElectorRequest request)
        {
            // Validamos que haya una elección activa
            var election =
                await _electionRepository.GetActiveElectionAsync()
                ?? throw new BusinessException(
                    "No existe una elección activa.",
                    "Voting.NoActiveElection"
                );

            // Validamos la cedula  ingresada (Este valueObject encapsula la validacion del formato de la cedula)
            var identityDocument = IdentityDocument.Create(request.IdentityDocument);

            // Validamos que el ciudadano exista y esté activo
            var citizen =
                await _citizenRepository.GetByIdentityDocumentAsync(identityDocument.Value)
                ?? throw new ValidationBusinessException(
                    nameof(request.IdentityDocument),
                    "Ciudadano no encontrado.",
                    "Voting.CitizenNotFound"
                );

            // Si el ciudadano está inactivo, lo bloqueamos
            if (!citizen.IsActive)
                throw new BusinessException(
                    "El ciudadano se encuentra inactivo.",
                    "Voting.CitizenInactive"
                );

            // Si ya voto, lo bloqueamos
            if (await _voterParticipationRepository.HasAlreadyVotedAsync(citizen.Id, election.Id))
            {
                throw new BusinessException(
                    "Este ciudadano ya emitió su voto en la elección activa.",
                    "Voting.AlreadyVoted"
                );
            }

            // Si el archivo subido no es una imagen válida, lo bloqueamos
            if (!_fileService.IsImageValid(request.IdCardImage))
                throw new ValidationBusinessException(
                    nameof(request.IdCardImage),
                    "La imagen de la cédula no es válida o es muy pesada.",
                    "General.InvalidFile"
                );

            // Validamos la cedula usando OCR
            var ocrResult = await _ocrService.ProcessIdentityCardAsync(request.IdCardImage);

            // Si el OCR falla o determina que el documento no es válido, lo bloqueamos
            if (!ocrResult.IsSuccess || !ocrResult.IsDocumentValid)
                throw new ValidationBusinessException(
                    nameof(request.IdCardImage),
                    ocrResult.ErrorMessage ?? "No fue posible validar el documento en la imagen.",
                    "Voting.OcrInvalid"
                );

            // Si el número extraído por OCR no coincide con el ingresado, lo bloqueamos
            if (
                !string.Equals(
                    identityDocument.Value,
                    ocrResult.IdentityNumber?.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                throw new ValidationBusinessException(
                    nameof(request.IdCardImage),
                    "El número extraído no coincide con el ingresado.",
                    "Voting.OcrMismatch"
                );
            }

            // Invalidamos los codigos OTP viejos
            await _verificationCodeRepository.InvalidatePreviousCodesAsync(citizen.Id, election.Id);

            // Generamos un nuevo codigo OTP
            var otpCode = _codeGenerator.Generate(6, useAlphanumeric: false);
            var verificationCode = VerificationCode.Create(
                citizen.Id,
                election.Id,
                otpCode,
                _dateTimeProvider.UtcNow
            );

            // Lo persistimos
            await _verificationCodeRepository.AddAsync(verificationCode);
            await _unitOfWork.SaveChangesAsync();

            // Enviamos el codigo por correo
            var emailModel = new OtpVerificationModel(
                $"{citizen.FirstName} {citizen.LastName}",
                otpCode
            );
            var emailSent = await _emailService.SendEmailAsync(
                citizen.Email,
                "Su Código de Verificación Electoral",
                "OtpVerification",
                emailModel
            );

            if (!emailSent)
                throw new BusinessException(
                    "Fallo en el servicio de correos.",
                    "Voting.EmailFailed"
                );

            return true;
        }

        public async Task<VerifyCodeResponse> VerifyOtpAsync(VerifyCodeRequest request)
        {
            // El repositorio ya filtra por IsUsed == false y ExpirationDate > now
            var code = await _verificationCodeRepository.GetValidCodeAsync(
                request.CitizenId,
                request.ElectionId,
                request.Code
            );

            if (code == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.Code),
                    "Código inválido o expirado.",
                    "VerificationCode.Invalid"
                );
            }

            code.Use(_dateTimeProvider.UtcNow);

            _verificationCodeRepository.Update(code);
            await _unitOfWork.SaveChangesAsync();

            return new VerifyCodeResponse(IsValid: true, ErrorMessage: null);
        }

        public async Task<VoterBallotResponse> GetBallotAsync(Guid electionId)
        {
            var election =
                await _electionRepository.GetByIdAsync(electionId)
                ?? throw new BusinessException("Elección inválida.", "Voting.InvalidElection");

            if (!election.IsActive || election.Status != Domain.Enums.ElectionStatus.Active)
                throw new BusinessException(
                    "La elección no se encuentra activa.",
                    "Voting.InvalidElection"
                );

            var positions = await _positionRepository.GetAllAsync(
                new QueryOptions<ElectivePosition> { Filter = p => p.IsActive, IsTracking = false }
            );

            var assignments = await _assignmentRepository.GetAllAsync(
                new QueryOptions<CandidatePostAssignment>
                {
                    Filter = a => a.Position.IsActive && a.Candidate.IsActive && a.Party.IsActive,
                    Includes = new() { a => a.Candidate, a => a.Party },
                    IsTracking = false,
                }
            );

            var ballotPositions = positions
                .Select(p =>
                {
                    var candidatesForPosition = assignments
                        .Where(a => a.PositionId == p.Id)
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

                    return p.ToBallotPosition(candidatesForPosition);
                })
                .ToList();

            return election.ToVoterBallot(ballotPositions);
        }

        public async Task SubmitVoteAsync(SubmitVoteRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var election =
                    await _electionRepository.GetByIdAsync(request.ElectionId)
                    ?? throw new BusinessException("Elección inválida.", "Voting.InvalidElection");

                var citizen =
                    await _citizenRepository.GetByIdAsync(request.CitizenId)
                    ?? throw new BusinessException("Ciudadano inválido.", "Voting.InvalidCitizen");

                if (
                    await _voterParticipationRepository.HasAlreadyVotedAsync(
                        request.CitizenId,
                        request.ElectionId
                    )
                )
                {
                    throw new BusinessException(
                        "Doble intento de voto bloqueado.",
                        "Voting.AlreadyVoted"
                    );
                }

                var usedCode =
                    await _verificationCodeRepository.GetFirstOrDefaultAsync(
                        new QueryOptions<VerificationCode>
                        {
                            Filter = c =>
                                c.CitizenId == request.CitizenId
                                && c.ElectionId == request.ElectionId
                                && c.Code == request.VerificationCode
                                && c.IsUsed,
                            IsTracking = false,
                        }
                    )
                    ?? throw new BusinessException(
                        "Intento de voto sin OTP validado.",
                        "Voting.CodeNotUsed"
                    );

                var participation = VoterParticipation.Create(
                    request.CitizenId,
                    request.ElectionId,
                    citizen.IsActive,
                    election.IsActive
                );

                await _voterParticipationRepository.AddAsync(participation);

                var votes = request
                    .Selections.Select(selection =>
                        Vote.Create(
                            request.ElectionId,
                            selection.PositionId,
                            selection.CandidateId,
                            selection.PartyId
                        )
                    )
                    .ToList();

                await _voteRepository.AddRangeAsync(votes);

                await _unitOfWork.CommitAsync();

                var confirmationModel = new VoteConfirmationModel(
                    $"{citizen.FirstName} {citizen.LastName}",
                    election.Name,
                    participation.Id.ToString("D8"),
                    _dateTimeProvider.UtcNow.ToString("dd/MM/yyyy HH:mm")
                );

                await _emailService.SendEmailAsync(
                    citizen.Email,
                    $"Comprobante de Votación - {election.Name}",
                    "VoteConfirmation",
                    confirmationModel
                );
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
