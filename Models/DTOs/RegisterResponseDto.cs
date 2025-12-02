namespace PresupuestoMVC.Models.DTOs
{
    public class RegisterResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
