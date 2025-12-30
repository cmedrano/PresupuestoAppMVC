using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<CuentaResponseDto>> GetAllUsersAsync();
    }
}
