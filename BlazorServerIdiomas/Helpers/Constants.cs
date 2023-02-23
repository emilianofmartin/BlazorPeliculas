using System.Globalization;

namespace BlazorServerIdiomas.Helpers {
    public static class Constants {
        public static CultureInfo[] SupportedCultures = new[] {
            new CultureInfo("en-US"),
            new CultureInfo("es-US"),
            new CultureInfo("es-AR"),
            new CultureInfo("es"),
            new CultureInfo("us")
        };
    }
}
