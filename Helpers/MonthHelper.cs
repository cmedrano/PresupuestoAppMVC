using System.Globalization;

namespace PresupuestoMVC.Helpers
{
    public static class MonthHelper
    {
        public static string MonthName(int mes)
        {
            var culture = new CultureInfo("es-AR");
            return culture.TextInfo.ToTitleCase(
                culture.DateTimeFormat.GetMonthName(mes)
            );
        }
    }
}
