using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<CuentaResponseDto>> GetAllAccountAsync();
        Task<CuentaResponseDto> CreateAccountAsync(CreateAccountViewRequest accountRequest);
        Task<CuentaResponseDto> CreateIncomeAsync(CreateIncomeViewRequest income);
        Task<CuentaResponseDto> CreateTransferAsync(CreateTransferViewRequest transfer);
    }
}
