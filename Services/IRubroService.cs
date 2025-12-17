using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public interface IRubroService
    {
        Task<RubroResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<RubroResponseDto>> GetAllRubroAsync();
        Task<IEnumerable<RubroType>> GetAllRubroTypesAsync();
        Task<RubroResponseDto> CreateAsync(CreateRubroViewRequest createDto);
        Task<RubroResponseDto> UpdateAsync(int id, UpdateRubroViewRequest updateDto);
        Task<bool> DeleteAsync(int id);
        Task<PaginacionRespuestaDto<RubroResponseDto>> GetFiltradosAsync(FiltroRubroViewRequest filtro, int pagina, int tamañoPagina);
    }
}
