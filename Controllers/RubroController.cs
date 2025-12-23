using Microsoft.AspNetCore.Mvc;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services;
using System.Globalization;

namespace PresupuestoMVC.Controllers
{
    public class RubroController : Controller
    {
        private readonly IRubroService _rubroService;

        public RubroController(IRubroService rubroService)
        {
            _rubroService = rubroService;
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
                var rubros = await _rubroService.GetAllRubroTypesAsync();

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
                var filtro = new FiltroRubroViewRequest
                {
                    Mes = mesFiltro,
                    Anio = anioFiltro,
                    RubroTypeId = rubroTypeId,
                    Pagina = pagina,
                    TamañoPagina = tamañoPagina
                };


                // Obtener datos paginados y filtrados
                var resultadoPaginado = await _rubroService.GetFiltradosAsync(filtro, pagina, tamañoPagina);

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

                return View("Views/Rubros/Rubros.cshtml");
            }
            catch (Exception ex)
            {
                // Manejar error y redirigir a página de error
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }

        }


        [HttpPost]
        public async Task<IActionResult> Edit(int Id, UpdateRubroViewRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _rubroService.UpdateAsync(Id, model);

                TempData["Success"] = "Rubro actualizado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRubroViewRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("Index");
                }

                await _rubroService.CreateAsync(model);

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

                await _rubroService.DeleteAsync(id);

                TempData["Success"] = "Rubro eliminado correctamente";
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
