using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Repository.Interfaces;
using PresupuestoMVC.Services;
using PresupuestoMVC.Services.Interfaces;
using System.Security.Claims;

namespace PresupuestoMVC.Controllers
{
    public class GastoController : Controller
    {
        private readonly IGastoService _gastoService;
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;

        public GastoController(IGastoService gastoService, IBudgetService budgetService, ICategoryService categoryService)
        {
            _gastoService = gastoService;
            _budgetService = budgetService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? rubroTypeId = null, int? cuentaId = null, int pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                // Cargar datos para los dropdowns
                var rubros = await _categoryService.GetAllCategoriesAsync();
                var cuentas = await _gastoService.GetAllCuentasAsync();

                // Crear filtro para el servicio
                var filtro = new FiltroGastoViewRequest
                {
                    RubroTypeId = rubroTypeId,
                    CuentaId = cuentaId,
                    Pagina = pagina,
                    TamañoPagina = tamañoPagina
                };

                // Obtener datos paginados y filtrados
                var resultadoPaginado = await _gastoService.GetFiltradosAsync(filtro, pagina, tamañoPagina);

                // Pasar datos a la vista
                ViewBag.Rubros = rubros;
                ViewBag.Cuentas = cuentas;
                ViewBag.Data = resultadoPaginado.Datos;
                ViewBag.Paginacion = resultadoPaginado;
                ViewBag.FiltroRubroId = rubroTypeId;
                ViewBag.FiltroCuentaId = cuentaId;
                ViewBag.PaginaActual = pagina;
                ViewBag.TamañoPagina = tamañoPagina;

                return View("Views/Diary/Diary.cshtml");
            }
            catch (Exception ex)
            {
                // Manejar error y redirigir a página de error
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateGastoViewRequest updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _gastoService.UpdateAsync(updateDto);

                TempData["Success"] = "Gasto actualizado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(CreateGastoViewRequest model)
        {
            try
            {
                int userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

                int companyId = int.Parse(User.FindFirst("CompanyId").Value);

                model.CreateByUserId = userId;
                model.CompanyId = companyId;

                var result = await _gastoService.CreateAsync(model);

                if (result.CreateGastoResult.ConfirmationRequired)
                {
                    return Json(new
                    {
                        ConfirmationRequired = true,
                        mensaje = result.CreateGastoResult.Message
                    });
                }
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Diary")
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGasto(int gastoId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _gastoService.DeleteGastoAsync(gastoId);

                TempData["Success"] = "Gasto eliminado correctamente";
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
