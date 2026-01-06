using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Services.Interfaces;

namespace PresupuestoMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _AccountService;
        public AccountController(IAccountService accountService)
        {
            _AccountService = accountService;
        }
        public async Task<IActionResult> Index()
        {

            try
            {
                var accounts = await _AccountService.GetAllUsersAsync();
                ViewBag.Accounts = accounts;

                return View("Views/Account/Account.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
