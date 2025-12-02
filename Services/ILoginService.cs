using PresupuestoMVC.Models.DTOs;
using PresupuestoMVC.Models.ViewModels;

namespace PresupuestoMVC.Services
{
    public interface ILoginService
    {
        Task<LoginResponseDto> LoginAsync(LoginViewRequest loginRequest);
        Task RevokeRefreshTokenAsync(string token);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshViewRequest request);
        Task<RegisterResponseDto> RegisterAsync(RegisterViewRequest registerRequest);
    }
}
