using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Data;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
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

            var usersDto = users.Select(x => new UserResponseDTO()
            {
                Id = x.Id,
                UserName = x.UserName,
                UserEmail = x.UserEmail,
                Rol = x.Role

            }).ToList();
            return usersDto;
        }

        public async Task<UserResponseDTO> CreateUserAsync(User userDto)
        {
            var userExiste = await _context.Users
                .AnyAsync(u => u.UserName == userDto.UserName || u.UserEmail == userDto.UserEmail);

            if (userExiste)
                throw new InvalidOperationException("El nombre de usuario o correo electrónico ya está en uso.");

            _context.Users.Add(userDto);
            await _context.SaveChangesAsync();
            var createdUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userDto.UserName);

            return new UserResponseDTO
            {
                UserName = createdUser.UserName,
                UserEmail = createdUser.UserEmail,
                Created = createdUser.CreateDate
            };
        }
    }
}
