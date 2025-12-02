namespace PresupuestoMVC.Models.Entities
{
    public class Rubro
    {
        public int Id { get; set; }
        public int RubroTypeId { get; set; } // Foreign Key
        public int valorInicial { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }

        // Navegación
        public RubroType tipoRubro { get; set; }
    }
}
