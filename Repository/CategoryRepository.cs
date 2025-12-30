using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Repository.Interfaces;

namespace PresupuestoMVC.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public CategoryRepository(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.RubroType.ToListAsync();

            var categoriesDto = categories.Select(x => new CategoryResponseDto()
            {
                Id = x.Id,
                nombreRubro= x.nombreRubro
            }).ToList();

            return categoriesDto;
        }
    }
}
