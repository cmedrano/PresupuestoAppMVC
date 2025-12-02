namespace PresupuestoMVC.Models.DTOs
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
