using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public class GastoService : IGastoService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GastoService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<GastoResponseDto> GetByIdAsync(int id)
        {
            var gasto = await _context.Gastos
                .Include(g => g.RubroType)
                .Include(g => g.Cuenta)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (gasto == null)
                throw new Exception($"Gasto con ID {id} no encontrado.");

            return _mapper.Map<GastoResponseDto>(gasto);
        }

        public async Task<IEnumerable<GastoResponseDto>> GetAllGastosAsync()
        {
            var gasto = await _context.Gastos
                .Include (g => g.RubroType)
                .Include (g => g.Cuenta)
                .OrderBy(g => g.Id)
                .ToListAsync();
            return _mapper.Map<IEnumerable<GastoResponseDto>>(gasto);
        }

        public async Task<GastoResponseDto> CreateAsync(CreateGastoViewRequest createDto)
        {
            if (createDto == null)
                throw new Exception($"El gasto no puede ser nulo." + nameof(createDto));

            var existeRubro = await _context.RubroType.AnyAsync(r => r.Id == createDto.RubroTypeId);
            if (!existeRubro)
                throw new Exception($"Rubro con ID {createDto.RubroTypeId} no existe.");

            var existeCuenta = await _context.Cuentas.AnyAsync(c => c.Id == createDto.CuentaId);
            if (!existeCuenta)
                throw new Exception($"Cuenta con ID {createDto.CuentaId} no existe.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var cuenta = await _context.Cuentas
                    .FirstOrDefaultAsync(c => c.Id == createDto.CuentaId);

                if (cuenta == null)
                    throw new Exception("Cuenta no encontrada");

                if (cuenta.SaldoActual < createDto.Monto)
                    throw new Exception("Saldo insuficiente");

                var gasto = _mapper.Map<Gasto>(createDto);

                _context.Gastos.Add(gasto);

                cuenta.SaldoActual -= createDto.Monto;

                var fecha = createDto.Fecha;
                int mes = fecha.Month;
                int anio = fecha.Year;

                var rubro = await _context.Budget.FirstOrDefaultAsync(r =>
                    r.RubroTypeId == createDto.RubroTypeId &&
                    r.Mes == mes &&
                    r.Anio == anio
                );

                if (rubro == null)
                    throw new Exception("No existe un rubro para el mes/año del gasto");

                rubro.ValorGastado += createDto.Monto;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await _context.Gastos
                    .Include(g => g.RubroType)
                    .Include(g => g.Cuenta)
                    .FirstOrDefaultAsync(g => g.Id == gasto.Id);

                return _mapper.Map<GastoResponseDto>(result);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GastoResponseDto> UpdateAsync(int id, UpdateGastoViewRequest updateDto)
        {
            // Validar existencia
            var existingGasto = await _context.Gastos
                .Include(g => g.RubroType)
                .Include(g => g.Cuenta)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (existingGasto == null)
                throw new Exception($"Gasto con ID {id} no encontrado.");

            var existeRubro = await _context.Budget.AnyAsync(r => r.Id == updateDto.RubroTypeId);
            if (!existeRubro)
                throw new Exception($"Rubro con ID {updateDto.RubroTypeId} no existe.");

            var existeCuenta = await _context.Cuentas.AnyAsync(c => c.Id == updateDto.CuentaId);
            if (!existeCuenta)
                throw new Exception($"Cuenta con ID {updateDto.CuentaId} no existe.");

            // Actualizar
            existingGasto.RubroTypeId = updateDto.RubroTypeId;
            existingGasto.CuentaId = updateDto.CuentaId;
            existingGasto.Fecha = updateDto.Fecha;
            existingGasto.Monto = updateDto.Monto;
            existingGasto.Nota = updateDto.Nota;

            await _context.SaveChangesAsync();

            var result = await _context.Gastos
                .Include(g => g.RubroType)
                .Include(g => g.Cuenta)
                .FirstOrDefaultAsync(g => g.Id == existingGasto.Id);

            return _mapper.Map<GastoResponseDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gastoExiste = await _context.Gastos.FindAsync(id);

            if (gastoExiste == null)
                throw new Exception($"Gasto con ID {id} no encontrado.");

            _context.Gastos.Remove(gastoExiste);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CuentaResponseDto>> GetAllCuentasAsync()
        {
            var cuentas = await _context.Cuentas
                .OrderBy(c => c.nombreCuenta)
                .ToListAsync();
            return _mapper.Map<List<CuentaResponseDto>>(cuentas);
        }

        public async Task<PaginacionRespuestaDto<GastoResponseDto>> GetFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina)
        {
            // Validar parámetros de paginación
            if (filtro.Pagina < 1)
                throw new Exception("La página debe ser mayor a 0.");

            if (filtro.TamañoPagina < 1 || filtro.TamañoPagina > 100)
                throw new Exception("El tamaño de página debe estar entre 1 y 100.");

            // Validar que el RubroTypeId existe
            if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
            {
                var tipoExiste = await _context.RubroType.AnyAsync(rt => rt.Id == filtro.RubroTypeId.Value);
                if (!tipoExiste)
                    throw new Exception($"Tipo de rubro con ID {filtro.RubroTypeId} no existe.");
            }

            // Obtener datos filtrados y paginados
            var query = _context.Gastos
                .Include(g => g.RubroType)
                .Include(g => g.Cuenta)
                .AsQueryable();

            // Aplicar filtros
            if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
            {
                query = query.Where(g => g.RubroTypeId == filtro.RubroTypeId.Value);
            }

            if (filtro.CuentaId.HasValue && filtro.CuentaId.Value > 0)
            {
                query = query.Where(g => g.CuentaId == filtro.CuentaId.Value);
            }

            // Obtener total de registros
            var totalRegistros = await query.CountAsync();

            // Aplicar paginación
            var gastos = await query
                .OrderByDescending(g => g.Fecha)
                .ThenBy(g => g.Id)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            var respuesta = new PaginacionRespuestaDto<GastoResponseDto>
            {
                Datos = _mapper.Map<List<GastoResponseDto>>(gastos),
                PaginaActual = filtro.Pagina,
                TamañoPagina = filtro.TamañoPagina,
                TotalRegistros = totalRegistros
            };

            return respuesta;
        }



    }
}
