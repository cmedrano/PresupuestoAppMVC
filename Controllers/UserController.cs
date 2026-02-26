using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services.Interfaces;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PresupuestoMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                ViewBag.Users = users;
                ViewBag.Roles = Enum.GetValues(typeof(UserRol))
                     .Cast<UserRol>()
                     .Select(r => new SelectListItem
                     {
                         Value = ((int)r).ToString(),
                         Text = r.ToString()
                     })
                     .ToList();

                return View("Views/Users/Users.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }

        }

        [Authorize(Roles = nameof(UserRol.Administrador))]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewRequest userRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                int companyId = int.Parse(User.FindFirst("CompanyId").Value);
                userRequest.CompanyId = companyId;

                var result = await _userService.CreateUserAsync(userRequest);
                if (result != null)
                {
                    TempData["Success"] = "Usuario creado correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "Usuario no fue creado";
                    return RedirectToAction("Index");
                }
            }

            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = nameof(UserRol.Administrador))]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            try
            {
                int userId = int.Parse(
                     User.FindFirstValue(ClaimTypes.NameIdentifier)
                 );

                var result = await _userService.ResetPassword(email, userId);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
