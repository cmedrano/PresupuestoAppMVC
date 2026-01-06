namespace PresupuestoMVC.Models.DTOs
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string nombreRubro { get; set; }
        public List<CategoryResponseDto>? SubCategories { get; set; } = new();
    }
}
