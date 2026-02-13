namespace PresupuestoMVC.Models.Entities
{
    public class Cuenta
    {
        public int Id { get; set; }
        public string nombreCuenta { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal SaldoActual { get; set; }
        public int CompanyId { get; set; }
    }
}
