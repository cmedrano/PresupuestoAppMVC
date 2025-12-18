using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
    }
}
