using PresupuestoMVC.Enums;

namespace PresupuestoMVC.Models.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; }
        public UserRol Rol { get; set; } = (UserRol)2;
        public DateTime Created { get; set; }
    }
}
