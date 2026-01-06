namespace PresupuestoMVC.Models.ViewModels
{
    public class UpdateBudgetViewRequest
    {
        public int RubroTypeId { get; set; }
        public int valorInicial { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
