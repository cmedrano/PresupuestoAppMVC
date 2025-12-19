using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    }
}
