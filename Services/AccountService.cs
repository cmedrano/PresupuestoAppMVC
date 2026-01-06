using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services.Interfaces;

namespace PresupuestoMVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<CuentaResponseDto>> GetAllUsersAsync()
        {
            return await _accountRepository.GetAllAccountAsync();
        }
    }
}
