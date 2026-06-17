using eVote360_Pro.Application.DTOs.User.Requests;
using eVote360_Pro.Application.DTOs.User.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Application.Models.Emails;
using eVote360_Pro.Domain.Common;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;
using eVote360_Pro.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace eVote360_Pro.Application.Services
{
    /// <summary>
    /// Implementación del servicio de usuarios.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IElectionRepository _electionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserService(
            IUserRepository userRepository,
            IElectionRepository electionRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            IConfiguration configuration
        )
        {
            _userRepository = userRepository;
            _electionRepository = electionRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var options = new QueryOptions<User>
            {
                Includes = new List<System.Linq.Expressions.Expression<Func<User, object>>>
                {
                    u => u.Role!,
                    u => u.LeaderAssignment!.Party,
                },
                IsTracking = false,
            };

            var users = await _userRepository.GetAllAsync(options);
            return users.ToResponse();
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(
                id,
                u => u.Role!,
                u => u.LeaderAssignment!.Party
            );
            return user?.ToResponse();
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                if (await _userRepository.ExistsByUsernameAsync(request.Username))
                {
                    throw new ValidationBusinessException(
                        nameof(request.Username),
                        "El nombre de usuario ya está registrado.",
                        "User.UsernameAlreadyExists"
                    );
                }

                if (await _userRepository.ExistsByEmailAsync(request.Email))
                {
                    throw new ValidationBusinessException(
                        nameof(request.Email),
                        "El correo electrónico ya está registrado.",
                        "User.EmailAlreadyExists"
                    );
                }

                PasswordPolicy.Create(request.Password);
                var passwordHash = _passwordHasher.Hash(request.Password);

                var user = User.Create(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Username,
                    passwordHash,
                    request.RoleId
                );

                await _userRepository.AddAsync(user);
                await _unitOfWork.CommitAsync();

                var welcomeModel = new UserWelcomeModel(
                    $"{user.FirstName} {user.LastName}",
                    user.Username,
                    request.Password,
                    _configuration["SiteSettings:LoginUrl"] ?? "https://evote360.com/auth/login"
                );

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Bienvenido a eVote360 Pro - Credenciales de Acceso",
                    "UserWelcome",
                    welcomeModel
                );

                return await GetByIdAsync(user.Id) ?? user.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var user =
                    await _userRepository.GetByIdAsync(request.Id, u => u.LeaderAssignment!)
                    ?? throw new BusinessException("Usuario no encontrado.", "User.NotFound");

                if (await _userRepository.ExistsByUsernameAsync(request.Username, request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.Username),
                        "El nombre de usuario ya está en uso.",
                        "User.UsernameAlreadyExists"
                    );
                }

                if (await _userRepository.ExistsByEmailAsync(request.Email, request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.Email),
                        "El correo electrónico ya está en uso.",
                        "User.EmailAlreadyExists"
                    );
                }

                user.UpdateInformation(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Username
                );

                bool hasAssignedParty = user.LeaderAssignment != null;
                bool isLastAdmin = await _userRepository.IsLastActiveAdminAsync(user.Id);
                user.UpdateRole(request.RoleId, hasAssignedParty, isLastAdmin);

                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    PasswordPolicy.Create(request.Password);
                    var newPasswordHash = _passwordHasher.Hash(request.Password);
                    user.UpdatePassword(newPasswordHash);
                }

                if (user.IsActive != request.IsActive)
                {
                    if (!request.IsActive)
                    {
                        bool isSelf = _currentUserService.UserId == user.Id;
                        user.Deactivate(isLastAdmin, isSelf);
                    }
                    else
                    {
                        user.Activate();
                    }
                }

                _userRepository.Update(user);
                await _unitOfWork.CommitAsync();

                return await GetByIdAsync(user.Id) ?? user.ToResponse();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task ToggleStatusAsync(Guid id, bool activate)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await EnsureNoActiveElectionAsync();

                var user =
                    await _userRepository.GetByIdAsync(id)
                    ?? throw new BusinessException("Usuario no encontrado.", "User.NotFound");

                if (!activate)
                {
                    bool isLastAdmin = await _userRepository.IsLastActiveAdminAsync(id);
                    bool isSelf = _currentUserService.UserId == id;
                    user.Deactivate(isLastAdmin, isSelf);
                }
                else
                {
                    user.Activate();
                }

                _userRepository.Update(user);
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
            {
                throw new BusinessException(
                    "No se permiten realizar cambios en los usuarios mientras exista una elección activa.",
                    "Election.ActiveAlreadyExists"
                );
            }
        }
    }
}
