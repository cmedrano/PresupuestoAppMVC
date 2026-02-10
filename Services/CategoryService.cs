using Microsoft.AspNetCore.Identity;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services.Interfaces;
using System.ComponentModel.Design;

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
            int? rubroPadreId = CreateDto.RubroPadreId == 0
                                ? null
                                : CreateDto.RubroPadreId;

            var CategoryDto = new RubroType
            {
                nombreRubro = CreateDto.Rubro ?? CreateDto.SubCategory,
                RubroPadreId = rubroPadreId,
                CompanyId = CreateDto.CompanyId
            };
            return await _categoryRepository.CreateAsync(CategoryDto);
        }
    }
}
