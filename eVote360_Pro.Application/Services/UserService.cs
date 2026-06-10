using eVote360_Pro.Application.DTOs.User.Requests;
using eVote360_Pro.Application.DTOs.User.Responses;
using eVote360_Pro.Application.Extensions;
using eVote360_Pro.Application.Interfaces.Services;
using eVote360_Pro.Domain.Entities;
using eVote360_Pro.Domain.Exceptions;
using eVote360_Pro.Domain.Interfaces.Persistence;
using eVote360_Pro.Domain.Interfaces.Repositories;
using eVote360_Pro.Domain.Interfaces.Security;

namespace eVote360_Pro.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICurrentUserService _currentUserService;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.ToResponse();
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user?.ToResponse();
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            // Validar si el Username ya existe
            if (await _userRepository.ExistsByUsernameAsync(request.Username))
            {
                throw new BusinessException("El nombre de usuario ya está registrado.", "User.UsernameAlreadyExists");
            }

            // Validar si el Email ya existe
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new BusinessException("El correo electrónico ya está registrado.", "User.EmailAlreadyExists");
            }

            // Hashear el password
            var passwordHash = _passwordHasher.Hash(request.Password);

            // Crear la entidad de dominio
            var user = User.Create(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Username,
                passwordHash,
                request.RoleId
            );

            // Persistir cambios
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user.ToResponse();
        }

        public async Task<UserResponse> UpdateAsync(UpdateUserRequest request)
        {
            var user = await _userRepository.GetByIdAsync(request.Id) 
                ?? throw new BusinessException("Usuario no encontrado.", "User.NotFound");

            // Validar unicidad de Username (excluyendo el usuario actual)
            if (await _userRepository.ExistsByUsernameAsync(request.Username, request.Id))
            {
                throw new BusinessException("El nombre de usuario ya está en uso por otro usuario.", "User.UsernameAlreadyExists");
            }

            // Validar unicidad de Email (excluyendo el usuario actual)
            if (await _userRepository.ExistsByEmailAsync(request.Email, request.Id))
            {
                throw new BusinessException("El correo electrónico ya está en uso por otro usuario.", "User.EmailAlreadyExists");
            }

            // Actualizar información básica
            user.UpdateInformation(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Username
            );

            // Actualizar password solo si se proporciona uno nuevo
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var newPasswordHash = _passwordHasher.Hash(request.Password);
                user.UpdatePassword(newPasswordHash);
            }

            // Manejar cambio de estado si es necesario
            if (user.IsActive != request.IsActive)
            {
                await ToggleStatusAsync(user.Id, request.IsActive);
            }
            else
            {
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();
            }

            return user.ToResponse();
        }

        public async Task ToggleStatusAsync(Guid id, bool activate)
        {
            var user = await _userRepository.GetByIdAsync(id) 
                ?? throw new BusinessException("Usuario no encontrado.", "User.NotFound");

            if (!activate)
            {
                // Obtener ID del usuario logueado
                var currentUserId = _currentUserService.UserId;
                
                // Verificar si es el último administrador activo
                var isLastAdmin = await _userRepository.IsLastActiveAdminAsync(id);
                
                // El chequeo de auto-desactivación y último admin se delega a la entidad para mantener la integridad del dominio
                user.Deactivate(isLastAdmin, id == currentUserId);
            }
            else
            {
                user.Activate();
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
