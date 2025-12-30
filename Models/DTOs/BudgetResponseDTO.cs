namespace PresupuestoMVC.Models.DTOs
{
    public class BudgetResponseDTO
    {
        public int Id { get; set; }
        public int RubroTypeId { get; set; }
        public string tipoRubroNombre { get; set; }
        public decimal valorInicial { get; set; }
        public decimal valorGastado { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
