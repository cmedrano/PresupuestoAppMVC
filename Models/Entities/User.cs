using PresupuestoMVC.Enums;

namespace PresupuestoMVC.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPasswordHash { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        //public UserRol? Rol { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
