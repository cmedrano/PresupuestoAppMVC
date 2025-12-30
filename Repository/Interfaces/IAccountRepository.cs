using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<CuentaResponseDto>> GetAllAccountAsync();
    }
}
