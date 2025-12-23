namespace PresupuestoMVC.Models.DTOs
{
    public class RubroResponseDto
    {
        public int Id { get; set; }
        public int RubroTypeId { get; set; }
        public string tipoRubroNombre { get; set; }
        public int valorInicial { get; set; }
        public int valorGastado { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
}
