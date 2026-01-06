using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Repository.Interfaces;

namespace PresupuestoMVC.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public AccountRepository(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<CuentaResponseDto>> GetAllAccountAsync()
        {
            var accounts = await _context.Cuentas
                .AsNoTracking()
                .OrderBy(c => c.nombreCuenta)
                .ToListAsync();

            var accountsDto = accounts.Select(x => new CuentaResponseDto()
            {
                Id = x.Id,
                nombreCuenta = x.nombreCuenta,
                SaldoActual = x.SaldoActual
                
            }).ToList();
            return accountsDto;
        }
    }
}
