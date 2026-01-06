using Microsoft.AspNetCore.Identity;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services.Interfaces;

namespace PresupuestoMVC.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }
        public async Task<CategoryResponseDto> CreateAsync(CreateCategoryViewRequest CreateDto)
        {
            var CategoryDto = new RubroType
            {
                nombreRubro = CreateDto.Rubro ?? CreateDto.SubCategory,
                RubroPadreId = CreateDto.RubroPadreId,
            };
            return await _categoryRepository.CreateAsync(CategoryDto);
        }
    }
}
