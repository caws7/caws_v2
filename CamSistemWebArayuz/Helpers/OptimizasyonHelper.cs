using System.Linq;

namespace CamSistemWebArayuz.Helpers
{
    public static class OptimizasyonHelper
    {
        /// <summary>
        /// Formats a comma-separated list of cut measurements into a compressed format.
        /// Example: "1038,1038,1038,1038,1038" -> "5x1038"
        /// Example: "1038,1038,1102,1102,1200" -> "2x1102, 2x1038, 1x1200"
        /// </summary>
        public static string FormatKesimOlculeri(string olcular)
        {
            if (string.IsNullOrWhiteSpace(olcular)) return "";
            var parts = olcular.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
            var groups = parts
                .GroupBy(x => x.Trim())
                .OrderByDescending(g => g.Count())
                .ThenBy(g => { int n; return int.TryParse(g.Key.Trim(), out n) ? n : int.MaxValue; })
                .Select(g => string.Format("{0}x{1}", g.Count(), g.Key));
            return string.Join(", ", groups);
        }
    }
}
