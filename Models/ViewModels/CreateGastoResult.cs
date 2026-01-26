using PresupuestoMVC.Models.DTOs;

namespace PresupuestoMVC.Models.ViewModels
{
    public class CreateGastoResult
    {
        public bool ConfirmationRequired { get; set; }
        public string? Message { get; set; }
        public GastoResponseDto? Gasto { get; set; }
    }
}
