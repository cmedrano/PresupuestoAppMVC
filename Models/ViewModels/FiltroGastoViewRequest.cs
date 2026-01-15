namespace PresupuestoMVC.Models.ViewModels
{
    public class FiltroGastoViewRequest
    {
        public int? RubroTypeId { get; set; }
        public int? CuentaId { get; set; }
        public int Pagina { get; set; }
        public int TamañoPagina { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}
