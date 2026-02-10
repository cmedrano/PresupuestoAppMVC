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
        Task<GastoResponseDto> UpdateAsync(UpdateGastoViewRequest updateDto);
        Task<bool> DeleteGastoAsync(int gastoId);
        Task<IEnumerable<CuentaResponseDto>> GetAllCuentasAsync();
        Task<PaginacionRespuestaDto<GastoResponseDto>> GetFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina);

    }
}
