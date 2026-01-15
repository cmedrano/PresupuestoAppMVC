using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
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
            var categories = await _context.RubroType
                .Where(r => r.RubroPadreId == null)
                .Include(r => r.SubRubros)
                .OrderBy(r => r.nombreRubro)
                .ToListAsync();

            var categoriesDto = categories.Select(x => new CategoryResponseDto()
            {
                Id = x.Id,
                nombreRubro= x.nombreRubro,
                SubCategories = x.SubRubros.Select(sr => new CategoryResponseDto
                {
                    Id = sr.Id,
                    nombreRubro = sr.nombreRubro
                }).ToList()
            }).ToList();

            return categoriesDto;
        }
        public async Task<CategoryResponseDto> CreateAsync(RubroType rubroTypeDto)
        {
            var userExiste = await _context.RubroType
                .AnyAsync(r => r.nombreRubro == rubroTypeDto.nombreRubro);

            if (userExiste)
                throw new InvalidOperationException("El nombre del rubro ya existe.");

            _context.RubroType.Add(rubroTypeDto);
            await _context.SaveChangesAsync();
            var createdCategory = await _context.RubroType
                .FirstOrDefaultAsync(r => r.nombreRubro == rubroTypeDto.nombreRubro);

            return new CategoryResponseDto
            {
                Id = createdCategory.Id,
                nombreRubro = createdCategory.nombreRubro
            };
        }
    }
}
