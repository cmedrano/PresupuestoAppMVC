namespace PresupuestoMVC.Models.ViewModels
{
    public class UpdateRubroViewRequest
    {
        public int RubroTypeId { get; set; }
        public int valorInicial { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
