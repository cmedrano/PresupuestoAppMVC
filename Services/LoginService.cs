using Konscious.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PresupuestoMVC.Data;
using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PresupuestoMVC.Services
{
    public class LoginService : ILoginService
    {
        private readonly AppDbContext _context;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiryMinutes;
        private readonly int _refreshTokenExpiryDays = 7; // Refresh token dura 7 días

        public LoginService(AppDbContext context ,IConfiguration configuration)
        {
            _context = context;
            _jwtKey = configuration["JwtSettings:Key"]!;
            _jwtIssuer = configuration["JwtSettings:Issuer"]!;
            _jwtAudience = configuration["JwtSettings:Audience"]!;
            _jwtExpiryMinutes = configuration.GetValue<int>("JwtSettings:ExpiryInMinutes");
        }

        public async Task<LoginResponseDto> LoginAsync(LoginViewRequest loginRequest)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == loginRequest.Username);

                // Verificar si el usuario existe
                if (user == null)
                    throw new UnauthorizedAccessException("Usuario incorrectos");

                // Verifico contraseña
                bool isPasswordValid = VerifyPassword(loginRequest.Password, user.UserPasswordHash);

                if (!isPasswordValid)
                    throw new UnauthorizedAccessException("Contraseña incorrectos");

                // Generar token JWT y RefreshToken para el usuario autenticado
                var token = GenerateJwtToken(user);
                var refreshToken = await GenerateRefreshToken(user.Id);

                // Retornar respuesta con el token JWT y información del usuario
                return new LoginResponseDto
                {
                    Token = token, // Token JWT generado
                    RefreshToken = refreshToken.Token, // Refresh token generado
                    Expiration = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes), // Fecha de expiración del token
                    Username = user.UserName // Nombre de usuario
                };
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // Método para verificar contraseñas con Argon2
        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Dividir el hash almacenado en salt y hash
                var parts = storedHash.Split(':');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

                // Calcular el hash de la contraseña proporcionada con la misma sal
                var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
                {
                    Salt = salt,
                    DegreeOfParallelism = 2,    // Reducir de 4 a 2
                    MemorySize = 32768,         // Reducir de 65536 a 32768 (32MB)
                    Iterations = 3              // Reducir de 4 a 3
                };

                byte[] computedHash = argon2.GetBytes(32);

                Console.WriteLine($"Argon2 verification took: {stopwatch.ElapsedMilliseconds}ms");

                // Comparar los hashes de manera segura
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
            }
            catch
            {
                return false;
            }
        }

        // Genera el JwtToken para ese Usuario
        private string GenerateJwtToken(User user)
        {
            // Crear manejador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // Obtener la clave secreta desde configuración y convertirla a bytes
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            // Configurar las propiedades del token JWT
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Definir los claims (información) que contendrá el token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName), // Claim con el nombre de usuario
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Claim con el ID del usuario
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("CompanyId", user.CompanyId.ToString())
                }),

                // Tiempo de expiración del token
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),

                // Quién emite el token (tu aplicación)
                Issuer = _jwtIssuer,

                // Para quién es válido el token
                Audience = _jwtAudience,

                // Credenciales de firma usando algoritmo HMAC SHA256
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear el token JWT basado en la configuración
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Convertir el token a string para devolverlo
            return tokenHandler.WriteToken(token);
        }

        // Generar Refresh Token
        private async Task<RefreshToken> GenerateRefreshToken(int userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays)
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return refreshToken;
        }

        // Refresh Token
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshViewRequest request)
        {
            var storedRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt =>
                    rt.Token == request.RefreshToken &&
                    !rt.IsRevoked &&
                    rt.ExpiresAt > DateTime.UtcNow);

            if (storedRefreshToken == null)
                throw new UnauthorizedAccessException("Refresh token inválido");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == storedRefreshToken.UserId);

            if (user == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Revocar el refresh token usado (rotación opcional)
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }

            // Generar nuevo access token
            var newAccessToken = GenerateJwtToken(user);

            // Generar NUEVO refresh token (rotación - más seguro)
            var newRefreshToken = await GenerateRefreshToken(user.Id);

            return new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                Expiration = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes)
            };
        }

        // Logout (revocar refresh token)
        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterViewRequest registerRequest)
        {
            // Validar si el usuario ya existe
            var userExiste = await _context.Users
                .AnyAsync(u => u.UserName == registerRequest.UserName || u.UserEmail == registerRequest.Email);

            if (userExiste)
                throw new InvalidOperationException("El nombre de usuario o correo electrónico ya está en uso.");

            // Hashear la contraseña
            var passwordHash = HashPassword(registerRequest.Password);

            // Crear nuevo usuario
            var user = new User
            {
                UserName = registerRequest.UserName,
                UserEmail = registerRequest.Email,
                UserPasswordHash = passwordHash,
                CreateDate = DateTime.UtcNow
            };

            // Guardar en la base de datos
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var createdUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == registerRequest.UserName);

            return new RegisterResponseDto
            {
                Message = "Usuario registrado exitosamente",
                UserName = createdUser.UserName,
                CreatedAt = createdUser.CreateDate
            };
        }

        // Método para hashear contraseñas con Argon2 (formato estándar)
        private string HashPassword(string password)
        {
            // Crear una sal (salt) aleatoria para mayor seguridad
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Configurar Argon2 con parámetros seguros
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 2,    // Reducir de 4 a 2
                MemorySize = 32768,         // Reducir de 65536 a 32768 (32MB)
                Iterations = 3              // Reducir de 4 a 3
            };

            // Generar el hash y devolver el formato estándar
            byte[] hash = argon2.GetBytes(32);

            // Para obtener el formato estándar, necesitamos construirlo manualmente
            // o usar una alternativa más simple
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

    }
}
