namespace PresupuestoMVC.Models.Entities
{
    public class Income
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public int CuentaId { get; set; }
        public Cuenta Cuenta { get; set; }
        public string? Note { get; set; }
    }
}
