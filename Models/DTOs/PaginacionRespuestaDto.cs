namespace PresupuestoMVC.Models.DTOs
{
    public class PaginacionRespuestaDto<T>
    {
        public List<T> Datos { get; set; } = new List<T>();
        public int PaginaActual { get; set; }
        public int TamañoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamañoPagina);
        public bool TieneAnterior => PaginaActual > 1;
        public bool TieneSiguiente => PaginaActual < TotalPaginas;
    }
}
