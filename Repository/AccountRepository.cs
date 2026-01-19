using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using System.Security.Cryptography.Xml;

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
        public async Task<CuentaResponseDto> CreateAccountAsync(Cuenta account)
        {
            var accountExiste = await _context.Cuentas
                .AnyAsync(r => r.nombreCuenta == account.nombreCuenta);

            if (accountExiste)
                throw new InvalidOperationException("El nombre de la cuenta ya existe.");

            _context.Cuentas.Add(account);
            await _context.SaveChangesAsync();
            var createdAccount = await _context.Cuentas
                .FirstOrDefaultAsync(r => r.nombreCuenta == account.nombreCuenta);

            return new CuentaResponseDto
            {
                Id = createdAccount.Id,
                nombreCuenta = createdAccount.nombreCuenta
            };
        }
        public async Task<CuentaResponseDto> CreateIncomeAsync(CreateIncomeViewRequest income)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var account = await _context.Cuentas
                    .FirstOrDefaultAsync(r => r.Id == income.Id);

                if (account == null)
                    throw new InvalidOperationException("la cuenta no existe.");

                account.SaldoActual += income.Amount;

                const int RUBRO_INGRESO_ID = 34;

                Income incomeDto = new Income()
                {
                    Amount = income.Amount,
                    Date = DateTime.UtcNow,
                    CuentaId = income.Id,
                    Note = income.Note,
                };

                _context.Income.Add(incomeDto);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new CuentaResponseDto
                {
                    Id = account.Id,
                    nombreCuenta = account.nombreCuenta,
                    SaldoActual = account.SaldoActual
                };
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<CuentaResponseDto> CreateTransferAsync(CreateTransferViewRequest transfer)
        {
            var accounts = await _context.Cuentas
            .Where(c => c.Id == transfer.AccountOriginId || c.Id == transfer.AccountDestinationId)
            .ToListAsync();

            var accountOrigin = accounts.FirstOrDefault(a => a.Id == transfer.AccountOriginId)
                ?? throw new Exception("Cuenta origen no encontrada");

            var accountDestination = accounts.FirstOrDefault(a => a.Id == transfer.AccountDestinationId)
                ?? throw new Exception("Cuenta destino no encontrada");

            if(accountOrigin.SaldoActual < transfer.Amount)
            {
                throw new Exception("Saldo insuficiente para realizar la operacion");
            }

            accountOrigin.SaldoActual -= transfer.Amount;
            accountDestination.SaldoActual += transfer.Amount;

            await _context.SaveChangesAsync();

            return new CuentaResponseDto
            {
                Id = accountOrigin.Id,
                nombreCuenta = accountOrigin.nombreCuenta,
                SaldoActual = accountOrigin.SaldoActual
            };
        }
    }
}
