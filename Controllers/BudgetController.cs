using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PresupuestoMVC.Enums;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services.Interfaces;
using System.Globalization;
using System.Security.Claims;

namespace PresupuestoMVC.Controllers
{
    public class BudgetController : Controller
    {
        private readonly IBudgetService _budgetService;
        private readonly ICategoryService _categoryService;

        public BudgetController(IBudgetService budgetService, ICategoryService categoryService)
        {
            _budgetService = budgetService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? rubroTypeId = null, int? mes = null, int? anio = null, int pagina = 1, int tamañoPagina = 10)
        {
            try
            {
                bool esPrimeraCarga = !Request.QueryString.HasValue;
                var today = DateTime.Now;
                int? mesFiltro = null;
                int? anioFiltro = null;
                if (esPrimeraCarga)
                {
                    mesFiltro = today.Month;
                    anioFiltro = today.Year;
                }
                else
                {
                    if (mes.HasValue && mes.Value != -1)
                        mesFiltro = mes;

                    if (anio.HasValue && anio.Value != -1)
                        anioFiltro = anio;
                }
                // Cargar datos para los dropdowns
                var rubros = await _categoryService.GetAllCategoriesAsync();

                // Años: 2025 + 5 años 2025-2030
                var anios = Enumerable.Range(2025, 6).ToList();

                var culture = new CultureInfo("es-AR");

                var meses = Enumerable.Range(1, 12)
                    .Select(m => new
                    {
                        Numero = m,
                        Nombre = culture.TextInfo.ToTitleCase(
                            culture.DateTimeFormat.GetMonthName(m).ToLower()
                        )
                    })
                    .ToList();

                // Crear filtro para el servicio
                var filtro = new FiltroBudgetViewRequest
                {
                    Mes = mesFiltro,
                    Anio = anioFiltro,
                    RubroTypeId = rubroTypeId,
                    Pagina = pagina,
                    TamañoPagina = tamañoPagina
                };


                // Obtener datos paginados y filtrados
                var resultadoPaginado = await _budgetService.GetFiltradosAsync(filtro, pagina, tamañoPagina);

                // Pasar datos a la vista
                ViewBag.Rubros = rubros;
                ViewBag.Meses = meses;
                ViewBag.Anios = anios;

                ViewBag.FiltroRubroId = rubroTypeId;
                ViewBag.FiltroMes = mesFiltro ?? -1;
                ViewBag.FiltroAnio = anioFiltro ?? -1;

                ViewBag.Data = resultadoPaginado.Datos;
                ViewBag.Paginacion = resultadoPaginado;
                ViewBag.PaginaActual = pagina;
                ViewBag.TamañoPagina = tamañoPagina;

                return View("Views/Budget/Budget.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }

        }


        [HttpPost]
        public async Task<IActionResult> Edit(int Id, UpdateBudgetViewRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _budgetService.UpdateAsync(Id, model);

                TempData["Success"] = "Rubro actualizado correctamente";
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
        public async Task<IActionResult> Create(CreateBudgetViewRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                int userId = int.Parse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier)
                );

                int companyId = int.Parse(User.FindFirst("CompanyId").Value);

                model.CreateByUserId = userId;
                model.CompanyId = companyId;

                await _budgetService.CreateAsync(model);

                TempData["Success"] = "Rubro creado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _budgetService.DeleteAsync(id);

                TempData["Success"] = "Rubro eliminado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesbyDate(DateTime date)
        {
            var categories = await _budgetService.GetCategoriesbyDateAsync(date);
            return Json(categories);
        }
    }
}
