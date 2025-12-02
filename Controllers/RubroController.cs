using Microsoft.AspNetCore.Mvc;
using PresupuestoMVC.Models.Entities;

namespace PresupuestoMVC.Controllers
{
    public class RubroController : Controller
    {
        public IActionResult Index()
        {
            return View("Views/Rubros/Rubros.cshtml");
        }
    }
}
