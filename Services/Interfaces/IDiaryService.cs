using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface IDiaryService
    {
        Task<IEnumerable<DiaryResponseDto>> GetAllDiaryAsync();
        //Task<PaginacionRespuestaDto<DiaryResponseDto>> GetDiaryFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina);
    }
}
