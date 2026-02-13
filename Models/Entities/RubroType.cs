namespace PresupuestoMVC.Models.Entities
{
    public class RubroType
    {
        public int Id { get; set; }
        public string nombreRubro { get; set; }

        public int? RubroPadreId { get; set; }
        public RubroType? RubroPadre { get; set; }
        public int CompanyId { get; set; }
        public ICollection<RubroType> SubRubros { get; set; } = new List<RubroType>();
    }
}
