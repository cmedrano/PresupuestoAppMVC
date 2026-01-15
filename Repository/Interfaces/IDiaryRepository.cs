using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface IDiaryRepository
    {
        Task<IEnumerable<DiaryResponseDto>> GetAllDiaryAsync();
        //Task<PaginacionRespuestaDto<GastoResponseDto>> GetFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina);
    }
}
