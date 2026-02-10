namespace PresupuestoMVC.Models.Entities
{
    public class Gasto
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
        public int CreateByUserId { get; set; }
        public User CreateByUser { get; set; }
        public int? UpdateByUserId { get; set; }
        public User? UpdateByUser { get; set; }
        public int? DeleteByUserId { get; set; }
        public User? DeleteByUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public int CompanyId { get; set; }
    }
}
