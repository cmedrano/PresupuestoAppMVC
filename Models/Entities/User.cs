using PresupuestoMVC.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PresupuestoMVC.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPasswordHash { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public UserRol Role { get; set; }
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
