using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Repository.Interfaces;

namespace PresupuestoMVC.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        public UserRepository(IMapper mapper, AppDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            // falta agregar el campo rol
            var usersDto = users.Select(x => new UserResponseDTO()
            {
                Id = x.Id,
                UserName = x.UserName,
                UserEmail = x.UserEmail

            }).ToList();
            return usersDto;
        }
    }
}
