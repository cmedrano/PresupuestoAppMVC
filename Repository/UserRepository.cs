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
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using PresupuestoMVC.Helpers;

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
        public async Task<bool> ResetPassword(string email, int userId, string randomPassword)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userExiste = await _context.Users
                  .AnyAsync(u => u.Id == userId);

                if (!userExiste)
                    throw new InvalidOperationException("El usuario no existe");

                var passwordHash = HashPasswordHelper.GetHashPassword(randomPassword);

                 await _context.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(u => u.UserPasswordHash, passwordHash)
                );

                var apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException("Falta RESEND_API_KEY");
                }

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var payload = new
                {
                    from = "dmarc@tupresupuestotest.online",
                    to = new[] { email },
                    subject = "📬 Test Solicitud cambio de contraseña",
                    html = $"<p>su nueva contraseña</p>" +
                           $"<p>{randomPassword}</p>"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await http.PostAsync("https://api.resend.com/emails", content);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;

            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
}
    }
}
