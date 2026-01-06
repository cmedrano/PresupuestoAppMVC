using PresupuestoMVC.Enums;

namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateUserViewRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserRol Rol { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
