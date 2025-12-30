using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    }
}
