using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace BlazorServerIdiomas.Pages {
    public class _HostModel: PageModel {
        public void OnGet() {
            //Agregamos una cookie con la estructura neceesaria.
            //HttpContext.Response.Cookies.Append(
            //    CookieRequestCultureProvider.DefaultCookieName,
            //    CookieRequestCultureProvider.MakeCookieValue(
            //        new RequestCulture(
            //            CultureInfo.CurrentCulture,
            //            CultureInfo.CurrentUICulture
            //        )
            //    )
            //);
        }
    }
}
