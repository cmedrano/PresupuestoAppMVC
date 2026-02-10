using PresupuestoMVC.Enums;
using System.ComponentModel.DataAnnotations;

namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateUserViewRequest
    {
        [RegularExpression(@"^\S+$", ErrorMessage = "No se permiten espacios")]
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserRol Rol { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
