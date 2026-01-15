using Microsoft.AspNetCore.Mvc;
using PresupuestoMVC.Models.Entities;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services.Interfaces;
using PresupuestoMVC.Services;

namespace PresupuestoMVC.Controllers
{
    public class DiaryController : Controller
    {
        private readonly IDiaryService _diaryService;
        private readonly ICategoryService _categoryService;
        private readonly IAccountService _accountService;
        private readonly IGastoService _gastoService;

        public DiaryController(IDiaryService diaryService, ICategoryService categodyService, IAccountService accountService, IGastoService gastoService)
        {
            _diaryService = diaryService;
            _categoryService = categodyService;
            _accountService = accountService;
            _gastoService = gastoService;
        }
        public async Task<IActionResult> Index(int? rubroTypeId = null, int? cuentaId = null, int pagina = 1, int tamañoPagina = 10, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            try
            {
                // Cargar datos para los dropdowns
                var rubros = await _categoryService.GetAllCategoriesAsync();
                var cuentas = await _accountService.GetAllAccountAsync();

                // Crear filtro para el servicio
                var filtro = new FiltroGastoViewRequest
                {
                    RubroTypeId = rubroTypeId,
                    CuentaId = cuentaId,
                    FechaDesde = fechaDesde,
                    FechaHasta = fechaHasta,
                    Pagina = pagina,
                    TamañoPagina = tamañoPagina
                };

                // Obtener datos paginados y filtrados
                var resultadoPaginado = await _gastoService.GetFiltradosAsync(filtro, pagina, tamañoPagina);

                // Pasar datos a la vista
                ViewBag.Rubros = rubros;
                ViewBag.Cuentas = cuentas;
                ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
                ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
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
    }
}
