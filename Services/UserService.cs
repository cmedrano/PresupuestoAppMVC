using Microsoft.AspNetCore.Identity.Data;
using PresupuestoMVC.Helpers;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services.Interfaces;

namespace PresupuestoMVC.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<UserResponseDTO> CreateUserAsync(CreateUserViewRequest userRequest)
        {
            var passwordHash = SecurityHelper.HashPassword(userRequest.Password);

            var userDto = new User
            {
                UserName = userRequest.UserName,
                UserEmail = userRequest.Email,
                UserPasswordHash = passwordHash,
                CompanyId = userRequest.CompanyId,
                Role = userRequest.Rol,
                CreateDate = DateTime.UtcNow
            };
            return await _userRepository.CreateUserAsync(userDto);
        }
        public async Task<bool> ResetPassword(string email, int userId)
        {
            var randomPassword = GeneratePassword.GenerateRandomPassword(10);
            return await _userRepository.ResetPassword(email, userId, randomPassword);
        }

    }
}
