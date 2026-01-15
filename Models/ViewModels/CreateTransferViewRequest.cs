namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateTransferViewRequest
    {
        public int AccountOriginId { get; set; }
        public int AccountDestinationId { get; set; }
        public int Amount { get; set; }
    }
}
