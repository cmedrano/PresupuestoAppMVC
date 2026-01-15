namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateIncomeViewRequest
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string? Note { get; set; }

    }
}
