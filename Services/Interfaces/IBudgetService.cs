using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<BudgetResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<BudgetResponseDTO>> GetAllBudgetAsync();
        Task<IEnumerable<RubroType>> GetAllRubroTypesAsync();
        Task<BudgetResponseDTO> CreateAsync(CreateBudgetViewRequest createDto);
        Task<BudgetResponseDTO> UpdateAsync(int id, UpdateBudgetViewRequest updateDto);
        Task<bool> DeleteAsync(int id);
        Task<PaginacionRespuestaDto<BudgetResponseDTO>> GetFiltradosAsync(FiltroBudgetViewRequest filtro, int pagina, int tamañoPagina);
        Task<IEnumerable<CategoryResponseDto>> GetCategoriesbyDateAsync(DateTime date);
    }
}
