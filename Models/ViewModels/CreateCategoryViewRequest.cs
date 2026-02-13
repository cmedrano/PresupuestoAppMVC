namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateCategoryViewRequest
    {
        public string Rubro { get; set; }
        public string SubCategory { get; set; }
        public int RubroPadreId { get; set; }
        public int CompanyId { get; set; }
    }
}
