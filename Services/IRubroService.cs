using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public interface IRubroService
    {
        Task<RubroResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<RubroResponseDTO>> GetAllRubroAsync();
        Task<IEnumerable<RubroType>> GetAllRubroTypesAsync();
        Task<RubroResponseDTO> CreateAsync(CreateRubroViewRequest createDto);
    }
}
