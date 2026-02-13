using Microsoft.AspNetCore.Mvc;
using PresupuestoMVC.Models.ViewModels;
using PresupuestoMVC.Services;
using PresupuestoMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace PresupuestoMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                ViewBag.Categories = categories;

                return View("Views/Category/Category.cshtml");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los datos: " + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewRequest model)
        {
            try
            {
                int companyId = int.Parse(User.FindFirst("CompanyId").Value);

                model.CompanyId = companyId;
                await _categoryService.CreateAsync(model);

                TempData["Success"] = "Rubro creado correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //[HttpDelete]
        //public async Task<ActionResult> DeleteCategory(DeleteCategoryViewRequest model)
        //{
        //    try
        //    {
        //        await _categoryService.CreateAsync(model);

        //        TempData["Success"] = "Rubro creado correctamente";
        //        return RedirectToAction("Index");
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["Error"] = "Error: " + ex.Message;
        //        return RedirectToAction("Index");
        //    }
        //}
    }
}
