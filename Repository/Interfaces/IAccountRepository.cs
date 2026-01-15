using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<CuentaResponseDto>> GetAllAccountAsync();
        Task<CuentaResponseDto> CreateAccountAsync(Cuenta account);
        Task<CuentaResponseDto> CreateIncomeAsync(CreateIncomeViewRequest income);
        Task<CuentaResponseDto> CreateTransferAsync(CreateTransferViewRequest transfer);
    }
}
