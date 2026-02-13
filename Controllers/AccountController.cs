using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services;
using PresupuestoMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

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
                var accounts = await _AccountService.GetAllAccountAsync();
                ViewBag.Accounts = accounts;

                return View("Views/Account/Account.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewRequest accountRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                int companyId = int.Parse(User.FindFirst("CompanyId").Value);
                accountRequest.CompanyId = companyId;

                await _AccountService.CreateAccountAsync(accountRequest);

                TempData["Success"] = "Cuenta creado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateIncome(CreateIncomeViewRequest incomeRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _AccountService.CreateIncomeAsync(incomeRequest);

                TempData["Success"] = "se ingreso correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransfer(CreateTransferViewRequest transferRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _AccountService.CreateTransferAsync(transferRequest);

                TempData["Success"] = "la transferencia se hizo correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
