namespace PresupuestoMVC.Models.Entities
{
    public class Diary
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string? Nota { get; set; }

        // Relaciones
        public int RubroTypeId { get; set; }
        public RubroType RubroType { get; set; }

        public int CuentaId { get; set; }
        public Cuenta Cuenta { get; set; }
    }
}
