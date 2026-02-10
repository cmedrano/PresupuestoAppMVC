namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateGastoViewRequest
    {
        public int RubroTypeId { get; set; }
        public int CuentaId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string? Nota { get; set; }
        public bool ForceNegativeBalance { get; set; } = false;
        public int CreateByUserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
