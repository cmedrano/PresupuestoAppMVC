using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services.Interfaces;
using System.Globalization;

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
    }
}
