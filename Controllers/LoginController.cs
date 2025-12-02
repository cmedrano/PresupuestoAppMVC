using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services;

namespace PresupuestoMVC.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si el usuario ya está autenticado, redirigir al home
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Login/Login - Procesa el formulario de login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(loginRequest);
            }

            try
            {
                var result = await _loginService.LoginAsync(loginRequest);

                // Guardar token en cookie (opcional) o session
                Response.Cookies.Append("X-Access-Token", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.Expiration
                });

                Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Redirigir al home después del login exitoso
                return RedirectToAction("Index", "Home");
            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(loginRequest);
            }
        }

        // GET: /Login/Register - Muestra el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View("Views/Register/Register.cshtml");
        }

        // POST: /Login/Register - Procesa el formulario de registro
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest);
            }

            try
            {
                var result = await _loginService.RegisterAsync(registerRequest);

                if (result != null)
                {
                    // Redirigir al login después del registro exitoso
                    TempData["SuccessMessage"] = "Registro exitoso. Por favor inicia sesión.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError(string.Empty, "Error en el registro");
                return View(registerRequest);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(registerRequest);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error interno del servidor");
                return View(registerRequest);
            }
        }

        // POST: /Login/Logout - Cerrar sesión
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Obtener refresh token de las cookies
            var refreshToken = Request.Cookies["X-Refresh-Token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _loginService.RevokeRefreshTokenAsync(refreshToken);
            }

            // Limpiar cookies
            Response.Cookies.Delete("X-Access-Token");
            Response.Cookies.Delete("X-Refresh-Token");

            return RedirectToAction("Login", "Login");
        }


    }
}
