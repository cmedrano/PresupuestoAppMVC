using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;

namespace PresupuestoMVC.Repository
{
    public class DiaryRepository : IDiaryRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DiaryRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IEnumerable<DiaryResponseDto>> GetAllDiaryAsync()
        {
            var diary = await _context.Diary
                .Include(g => g.RubroType)
                .Include(g => g.Cuenta)
                .OrderBy(g => g.Id)
                .ToListAsync();
            return _mapper.Map<IEnumerable<DiaryResponseDto>>(diary);
        }
        //public async Task<PaginacionRespuestaDto<GastoResponseDto>> GetFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina)
        //{
        //    // Validar parámetros de paginación
        //    if (filtro.Pagina < 1)
        //        throw new Exception("La página debe ser mayor a 0.");

        //    if (filtro.TamañoPagina < 1 || filtro.TamañoPagina > 100)
        //        throw new Exception("El tamaño de página debe estar entre 1 y 100.");

        //    // Validar que el RubroTypeId existe
        //    if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
        //    {
        //        var tipoExiste = await _context.RubroType.AnyAsync(rt => rt.Id == filtro.RubroTypeId.Value);
        //        if (!tipoExiste)
        //            throw new Exception($"Tipo de rubro con ID {filtro.RubroTypeId} no existe.");
        //    }

        //    // Obtener datos filtrados y paginados
        //    var query = _context.Gastos
        //        .Include(g => g.RubroType)
        //        .Include(g => g.Cuenta)
        //        .AsQueryable();

        //    // Aplicar filtros
        //    if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
        //    {
        //        query = query.Where(g => g.RubroTypeId == filtro.RubroTypeId.Value);
        //    }

        //    if (filtro.CuentaId.HasValue && filtro.CuentaId.Value > 0)
        //    {
        //        query = query.Where(g => g.CuentaId == filtro.CuentaId.Value);
        //    }

        //    // Obtener total de registros
        //    var totalRegistros = await query.CountAsync();

        //    // Aplicar paginación
        //    var gastos = await query
        //        .OrderByDescending(g => g.Fecha)
        //        .ThenBy(g => g.Id)
        //        .Skip((pagina - 1) * tamañoPagina)
        //        .Take(tamañoPagina)
        //        .ToListAsync();

        //    var respuesta = new PaginacionRespuestaDto<GastoResponseDto>
        //    {
        //        Datos = _mapper.Map<List<GastoResponseDto>>(gastos),
        //        PaginaActual = filtro.Pagina,
        //        TamañoPagina = filtro.TamañoPagina,
        //        TotalRegistros = totalRegistros
        //    };

        //    return respuesta;
        //}
    }
}
