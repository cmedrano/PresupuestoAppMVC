namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateBudgetViewRequest
    {
        public int rubroTypeId { get; set; }
        public int valorInicial { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateByUserId { get; set; }
        public int CompanyId { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
