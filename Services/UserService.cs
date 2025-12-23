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
                CreateDate = DateTime.UtcNow
            };
            return await _userRepository.CreateUserAsync(userDto);
        }

    }
}
