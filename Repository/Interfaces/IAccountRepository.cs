using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<CuentaResponseDto>> GetAllAccountAsync();
        Task<CuentaResponseDto> CreateAccountAsync(Cuenta account);
    }
}
