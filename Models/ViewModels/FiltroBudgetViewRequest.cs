namespace PresupuestoMVC.Models.ViewModels
{
    public class FiltroBudgetViewRequest
    {
        public int? RubroTypeId { get; set; }
        public int? Mes { get; set; }
        public int? Anio { get; set; }
        public int Pagina { get; set; }
        public int TamañoPagina { get; set; }
    }
}
