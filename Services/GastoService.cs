using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                createDto.CreateDate = DateTime.UtcNow;
                var cuenta = await _context.Cuentas
                    .FirstOrDefaultAsync(c => c.Id == createDto.CuentaId);

                if (cuenta == null)
                    throw new Exception("Cuenta no encontrada");

                if (cuenta.SaldoActual < createDto.Monto && !createDto.ForceNegativeBalance)
                {
                    return new GastoResponseDto
                    {
                        CreateGastoResult = new CreateGastoResult() { ConfirmationRequired = true, Message = "La cuenta no tiene saldo suficiente. ¿Desea continuar igualmente?" }
                    };
                }
                
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

                var response = _mapper.Map<GastoResponseDto>(result);

                response.CreateGastoResult = new CreateGastoResult
                {
                    ConfirmationRequired = false
                };

                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GastoResponseDto> UpdateAsync(UpdateGastoViewRequest updateDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var gasto = await _context.Gastos
                    .FirstOrDefaultAsync(g => g.Id == updateDto.Id);

                if (gasto == null)
                    throw new Exception("Gasto no encontrado");

                var cuentaAnterior = await _context.Cuentas
                    .FirstOrDefaultAsync(c => c.Id == gasto.CuentaId);

                var rubroAnterior = await _context.Budget
                    .FirstOrDefaultAsync(r =>
                        r.RubroTypeId == gasto.RubroTypeId &&
                        r.Mes == gasto.Fecha.Month &&
                        r.Anio == gasto.Fecha.Year);

                if (cuentaAnterior == null || rubroAnterior == null)
                    throw new Exception("Datos anteriores inválidos");

                cuentaAnterior.SaldoActual += gasto.Monto;
                rubroAnterior.ValorGastado -= gasto.Monto;

                var cuentaNueva = await _context.Cuentas
                    .FirstOrDefaultAsync(c => c.Id == updateDto.CuentaId);

                if (cuentaNueva == null)
                    throw new Exception("Cuenta nueva no encontrada");

                if (cuentaNueva.SaldoActual < updateDto.Monto)
                    throw new Exception("Saldo insuficiente");

                var rubroNuevo = await _context.Budget
                    .FirstOrDefaultAsync(r =>
                        r.RubroTypeId == updateDto.RubroTypeId &&
                        r.Mes == updateDto.Fecha.Month &&
                        r.Anio == updateDto.Fecha.Year);

                if (rubroNuevo == null)
                    throw new Exception("Rubro nuevo no encontrado");

                gasto.Monto = updateDto.Monto;
                gasto.Fecha = DateTime.SpecifyKind(updateDto.Fecha.Date, DateTimeKind.Utc); ;
                gasto.RubroTypeId = updateDto.RubroTypeId;
                gasto.CuentaId = updateDto.CuentaId;
                gasto.Nota = updateDto.Nota;

                cuentaNueva.SaldoActual -= updateDto.Monto;
                rubroNuevo.ValorGastado += updateDto.Monto;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var result = await _context.Gastos
                    .Include(g => g.RubroType)
                    .Include(g => g.Cuenta)
                    .FirstAsync(g => g.Id == gasto.Id);

                return _mapper.Map<GastoResponseDto>(result);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<bool> DeleteGastoAsync(int gastoId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var gasto = await _context.Gastos
                    .FirstOrDefaultAsync(g => g.Id == gastoId);

                if (gasto == null)
                    throw new Exception($"Gasto con ID {gastoId} no encontrado");

                var cuenta = await _context.Cuentas
                    .FirstOrDefaultAsync(c => c.Id == gasto.CuentaId);

                if (cuenta == null)
                    throw new Exception("Cuenta no encontrada");

                var fecha = gasto.Fecha;
                int mes = fecha.Month;
                int anio = fecha.Year;

                var rubro = await _context.Budget.FirstOrDefaultAsync(r =>
                    r.RubroTypeId == gasto.RubroTypeId &&
                    r.Mes == mes &&
                    r.Anio == anio
                );

                if (rubro == null)
                    throw new Exception("Rubro del gasto no encontrado");

                cuenta.SaldoActual += gasto.Monto;
                rubro.ValorGastado -= gasto.Monto;

                _context.Gastos.Remove(gasto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            try
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
                var queryGasto = _context.Gastos
                    .Include(g => g.RubroType)
                    .Include(g => g.Cuenta)
                    .Include(g => g.CreateByUser)
                    .Include(g => g.UpdateByUser)
                    .Include(g => g.DeleteByUser)
                    .AsQueryable();

                var queryIncome = _context.Income
                        .Include(g => g.Cuenta)
                .AsQueryable();

                // Aplicar filtros
                if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
                {
                    queryGasto = queryGasto.Where(g => g.RubroTypeId == filtro.RubroTypeId.Value);
                    //queryIncome = queryIncome.Where(g => g.RubroTypeId == filtro.RubroTypeId.Value);
                }

                if (filtro.CuentaId.HasValue && filtro.CuentaId.Value > 0)
                {
                    queryGasto = queryGasto.Where(g => g.CuentaId == filtro.CuentaId.Value);
                    queryIncome = queryIncome.Where(g => g.CuentaId == filtro.CuentaId.Value);
                }

                if (filtro.FechaDesde.HasValue)
                {
                    var desdeUtc = DateTime.SpecifyKind(
                    filtro.FechaDesde.Value.Date,
                    DateTimeKind.Utc
 );
                    queryGasto = queryGasto.Where(g => g.Fecha >= desdeUtc);
                    queryIncome = queryIncome.Where(g => g.Date >= desdeUtc);
                }

                if (filtro.FechaHasta.HasValue)
                {
                    var hastaUtc = DateTime.SpecifyKind(
                    filtro.FechaHasta.Value.Date.AddDays(1).AddTicks(-1),
                    DateTimeKind.Utc
                    );

                    queryGasto = queryGasto.Where(g => g.Fecha <= hastaUtc);
                    queryIncome = queryIncome.Where(g => g.Date <= hastaUtc);
                }

                var gastoDto = _mapper.Map<List<GastoResponseDto>>(queryGasto);
                //var incomeDto = _mapper.Map<List<GastoResponseDto>>(queryIncome);


                var datosUnificados = gastoDto
                    //.Concat(incomeDto)
                    .OrderByDescending(x => x.Fecha)
                    .ToList();

                var datosPaginados = datosUnificados
                    .Skip((filtro.Pagina - 1) * filtro.TamañoPagina)
                    .Take(filtro.TamañoPagina)
                    .ToList();

                // Obtener total de registros
                var totalRegistros = datosUnificados.Count;

                var respuesta = new PaginacionRespuestaDto<GastoResponseDto>
                {
                    Datos = datosPaginados,
                    PaginaActual = filtro.Pagina,
                    TamañoPagina = filtro.TamañoPagina,
                    TotalRegistros = totalRegistros
                };

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
