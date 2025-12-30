using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
    }
}
