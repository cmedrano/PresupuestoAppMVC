namespace PresupuestoMVC.Models.DTOs
{
    public class CuentaResponseDto
    {
        public int Id { get; set; }
        public string nombreCuenta { get; set; }
        public decimal SaldoActual { get; set; }
    }
}
