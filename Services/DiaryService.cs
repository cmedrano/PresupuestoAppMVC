using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services.Interfaces;

namespace PresupuestoMVC.Services
{
    public class DiaryService : IDiaryService
    {
        private readonly IDiaryRepository _diaryRepository;

        public DiaryService (IDiaryRepository diaryRepository)
        {
            _diaryRepository = diaryRepository;
        }
        public async Task<IEnumerable<DiaryResponseDto>> GetAllDiaryAsync()
        {
            return await _diaryRepository.GetAllDiaryAsync();
        }
        //public async Task<PaginacionRespuestaDto<DiaryResponseDto>> GetDiaryFiltradosAsync(FiltroGastoViewRequest filtro, int pagina, int tamañoPagina)
        //{
        //    return await _diaryRepository.GetDiaryFiltradosAsync(filtro, pagina, tamañoPagina);
        //}
    }
}
