namespace PresupuestoMVC.Models.Entities
{
    public class Budget
    {
        public int Id { get; set; }
        public int RubroTypeId { get; set; } // Foreign Key
        public decimal valorInicial { get; set; }
        public decimal ValorGastado { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public User CreateByUser { get; set; }
        public int CreateByUserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreateDate { get; set; }

        // Navegación
        public RubroType tipoRubro { get; set; }
    }
}
