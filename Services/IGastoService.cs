using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public interface IGastoService
    {
        Task<GastoResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<GastoResponseDto>> GetAllGastosAsync();
        Task<GastoResponseDto> CreateAsync(CreateGastoViewRequest createDto);
        Task<GastoResponseDto> UpdateAsync(int id, UpdateGastoViewRequest updateDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CuentaResponseDto>> GetAllCuentasAsync();
        Task<PaginacionRespuestaDto<GastoResponseDto>> GetFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina);

    }
}
