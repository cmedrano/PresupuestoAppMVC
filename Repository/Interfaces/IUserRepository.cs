using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> CreateUserAsync(User user);
        Task<bool> ResetPassword(string email, int userId, string randomPassword);
    }
}
