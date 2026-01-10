namespace PresupuestoMVC.Models.ViewModels
{
    public class UpdateGastoViewRequest
    {
        public int Id { get; set; }
        public int RubroTypeId { get; set; }
        public int CuentaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string? Nota { get; set; }
    }
}
