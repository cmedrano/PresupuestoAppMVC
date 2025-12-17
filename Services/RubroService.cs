using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public class RubroService : IRubroService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public RubroService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RubroResponseDto> GetByIdAsync(int id)
        {
            var rubro = await _context.Rubros
                .Include(r => r.tipoRubro)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (rubro == null)
                throw new Exception($"Rubro con ID {id} no encontrado.");

            return _mapper.Map<RubroResponseDto>(rubro);
        }

        public async Task<IEnumerable<RubroResponseDto>> GetAllRubroAsync()
        {
            var rubros = await _context.Rubros.ToListAsync();
            return _mapper.Map<IEnumerable<RubroResponseDto>>(rubros);
        }

        public async Task<IEnumerable<RubroType>> GetAllRubroTypesAsync()
        {
            var rubroTypes = await _context.RubroType
                .OrderBy(rt => rt.Id)
                .ToListAsync();
            return rubroTypes;
        }

        public async Task<RubroResponseDto> CreateAsync(CreateRubroViewRequest createDto)
        {
            // Validaciones
            var tipoExiste = await _context.RubroType.AnyAsync(rt => rt.Id == createDto.rubroTypeId);

            if (!tipoExiste)
                throw new Exception($"Tipo de rubro con ID {createDto.rubroTypeId} no existe.");

            if (createDto.valorInicial < 0)
                throw new Exception("El valor inicial no puede ser negativo.");

            if (createDto.Mes < 1 || createDto.Mes > 12)
                throw new Exception("El mes debe estar entre 1 y 12.");

            var rubro = _mapper.Map<Rubro>(createDto);

            _context.Rubros.Add(rubro);
            await _context.SaveChangesAsync();

            var result = await _context.Rubros
                .Include(r => r.tipoRubro)
                .FirstOrDefaultAsync(r => r.Id == rubro.Id);

            return _mapper.Map<RubroResponseDto>(result);
        }

        public async Task<RubroResponseDto> UpdateAsync(int id, UpdateRubroViewRequest updateDto)
        {
            // Validar existencia
            var existingRubro = await _context.Rubros
                .Include(r => r.tipoRubro)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (existingRubro == null)
                throw new Exception($"Rubro con ID {id} no encontrado.");

            // Validaciones
            var tipoExiste = await _context.RubroType.AnyAsync(rt => rt.Id == updateDto.RubroTypeId);

            if (!tipoExiste)
                throw new Exception($"Tipo de rubro con ID {updateDto.RubroTypeId} no existe.");

            if (updateDto.valorInicial < 0)
                throw new Exception("El valor inicial no puede ser negativo.");

            if (updateDto.Mes < 1 || updateDto.Mes > 12)
                throw new Exception("El mes debe estar entre 1 y 12.");

            // Actualizar
            existingRubro.RubroTypeId = updateDto.RubroTypeId;
            existingRubro.valorInicial = updateDto.valorInicial;
            existingRubro.Mes = updateDto.Mes;
            existingRubro.Anio = updateDto.Anio;

            var result = _context.Rubros.Update(existingRubro);
            await _context.SaveChangesAsync();

            return _mapper.Map<RubroResponseDto>(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existsRubro = await _context.Rubros.AnyAsync(g => g.Id == id);

            if (!existsRubro)
                throw new Exception($"Rubro con ID {id} no encontrado.");

            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro == null)
                return false;

            _context.Rubros.Remove(rubro);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RubroType>> GetTiposRubroAsync()
        {
            var tipos = await _context.RubroType
                .OrderBy(rt => rt.nombreRubro)
                .ToListAsync();

            return _mapper.Map<List<RubroType>>(tipos);
        }

        public async Task<PaginacionRespuestaDto<RubroResponseDto>> GetFiltradosAsync(FiltroRubroViewRequest filtro, int pagina, int tamañoPagina)
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
            var query = _context.Rubros
                .Include(r => r.tipoRubro)
                .AsQueryable();

            // Aplicar filtros
            if (filtro.Mes.HasValue && filtro.Mes.Value > 0)
            {
                query = query.Where(r => r.Mes == filtro.Mes.Value);
            }

            if (filtro.Anio.HasValue && filtro.Anio.Value > 0)
            {
                query = query.Where(r => r.Anio == filtro.Anio.Value);
            }

            if (filtro.RubroTypeId.HasValue && filtro.RubroTypeId.Value > 0)
            {
                query = query.Where(r => r.RubroTypeId == filtro.RubroTypeId.Value);
            }

            // Obtener total de registros
            var totalRegistros = await query.CountAsync();

            // Aplicar paginación
            var rubros = await query
                .OrderBy(r => r.Id)
                .ThenBy(r => r.Anio)
                .ThenBy(r => r.Mes)
                .ThenBy(r => r.tipoRubro.nombreRubro)
                .Skip((pagina - 1) * tamañoPagina)
                .Take(tamañoPagina)
                .ToListAsync();

            var respuesta = new PaginacionRespuestaDto<RubroResponseDto>
            {
                Datos = _mapper.Map<List<RubroResponseDto>>(rubros),
                PaginaActual = filtro.Pagina,
                TamañoPagina = filtro.TamañoPagina,
                TotalRegistros = totalRegistros
            };

            return respuesta;
        }
    }
}
