namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateAccountViewRequest
    {
        public string AccountName { get; set; }
        public int InitialBalance { get; set; }
        public int CompanyId { get; set; }
    }
}
