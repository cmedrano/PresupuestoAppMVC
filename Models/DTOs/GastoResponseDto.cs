namespace PresupuestoMVC.Models.DTOs
{
    public class GastoResponseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string? Nota { get; set; }

        // relaciones
        public int RubroTypeId { get; set; }
        public string RubroTypeNombre { get; set; }

        public int CuentaId { get; set; }
        public string CuentaNombre { get; set; }
    }
}
